using MediatR;
using TSMA.Meal.Web.ConfigModels;
using TSMA.Meal.Web.Features.Account.SendOtp.Exceptions;
using TSMA.Meal.Web.Features.Account.SendOtp.Sms;
using TSMA.Meal.Web.Shared.Exceptions;
using TSMA.Meal.Web.Shared.Repositories.Personnel;
using TSMA.Meal.Web.Shared.Services.Otp;
using TSMA.Meal.Web.Tools;

namespace TSMA.Meal.Web.Features.Account.SendOtp;

internal class SendOtpHandler(IPersonnelRepository repository, ISmsService smsService, IPersonnelRepository personnelRepository, IOtpService otpRepository) : IRequestHandler<SendOtpRequest>
{
    public async Task Handle(SendOtpRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.PhoneNumber) || !request.PhoneNumber.StartsWith("09")) throw new InvalidNumberException();

        //IF PERSONNEL EXIST
        var exist = await repository.ExistByPhoneAsync(request.PhoneNumber, cancellationToken);

        if (!exist) throw new PersonnelNotExistException();

        if (!await personnelRepository.PersonnelHasAccessByPhone(request.PhoneNumber, cancellationToken)) throw new NotPermittedException();


        if (otpRepository.ExistOtp(request.PhoneNumber))
        {
            throw new TrySendOtpAfterMinutesException(SecuritySettingsConfigModel.OtpExpiryMinutes);
        }


        var attemptCount = otpRepository.GetSendAttemptCount(request.PhoneNumber);

        if (attemptCount >= SecuritySettingsConfigModel.MaxOtpAttemptsPerDay) throw new TooManyOtpSentException();

        var code = new Random().Next(100000, 999999).ToString("D6");

        await smsService.SendOtpAsync(request.PhoneNumber, code);

        otpRepository.IncreaseSendAttempt(request.PhoneNumber, TimeSpan.FromHours(SecuritySettingsConfigModel.BanDurationHours));

        var encoded = Encoder.Encode(code);

        otpRepository.SetOtp(request.PhoneNumber, encoded, TimeSpan.FromMinutes(SecuritySettingsConfigModel.OtpExpiryMinutes));

        otpRepository.DeleteCheckAttempt(request.PhoneNumber);
    }
}

using TSMA_Meal.Shared.Exceptions;

namespace TSMA_Meal.Features.Account.SendOtp.Exceptions;

internal class TrySendOtpAfterMinutesException(int minutes) : BaseException($"لطفا بعد از {minutes} دقیقه تلاش کنید.");

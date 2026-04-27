using MediatR;
using TSMA_Meal.Features.Account;
using TSMA_Meal.Shared;
using TSMA_Meal.Tools;

namespace TSMA_Meal.Features.Meals.CancelReserve;


public class CancelReserveHandler(ICancelReserveRepository repository, IPersonnelRepository personnelRepository, ILogger<CancelReserveHandler> logger) : IRequestHandler<CancelReserveRequest>
{
    public async Task Handle(CancelReserveRequest request, CancellationToken cancellationToken)
    {
        //For Test
        //request = request with{Week =+ 5};

        if (request.ReserveId == 0) return;


        //IS PERSONNEL ACTIVE

        var personnel = await personnelRepository.GetPersonnelAsync(request.PersonnelCode, cancellationToken);

        if (personnel == null)
        {
            logger.LogCritical(LogMessages.WrongUserToCancelMeal(request.PersonnelCode));

            throw new PersonnelNotExistException();
        }



        //CHECK PERSONNEL ACCESS    

        if (!await personnelRepository.PersonnelHasAccess(request.PersonnelCode)) throw new NotPermittedException();



        //GET RESERVATION

        var reserve = await repository.GetReserveAsync(request.ReserveId, cancellationToken);

        if (reserve == null)
        {
            logger.LogCritical(LogMessages.UserTriedCancelWrongMeal(request.PersonnelCode));

            throw new UnknownException();
        }

        if (reserve.PersonnelCode != request.PersonnelCode)
        {
            logger.LogCritical(LogMessages.UserTriedCancelOthersMeal(request.PersonnelCode, reserve.PersonnelCode));

            throw new UnknownException();
        }



        //CAN RESERVE

        var today = PersianDateTime.GetPersianDateByWeekDay((int)DateTime.Now.DayOfWeek);

        //IF IS EXPIRED
        //EX: foodDate = "1403/01/10"، today = "1403/01/11" => ANSWER: TRUE => return FALSE
        if (string.CompareOrdinal(reserve.ReserveDate, today) <= 0) throw new CancelReserveOutOfDateException();



        //IF CANCELED BEFORE 

        if (reserve.CanceledReserve) return;



        //CANCEL RESERVE 

        reserve.CanceledReserve = true;

        await repository.UpdateReserveAsync(reserve, cancellationToken);
    }
}

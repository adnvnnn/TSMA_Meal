using MediatR;
using System.Globalization;
using TSMA_Meal.Database.Entities;
using TSMA_Meal.Features.Account;
using TSMA_Meal.Shared;
using TSMA_Meal.Tools;

namespace TSMA_Meal.Features.Meals.ReserveMeal;

public class ReserveFoodHandler(IOffDaysRepository offDaysRepository, IReserveFoodRepository repository, IPersonnelRepository personnelRepository, ILogger<ReserveFoodHandler> logger) : IRequestHandler<ReserveFoodRequest, int>
{
    public async Task<int> Handle(ReserveFoodRequest request, CancellationToken cancellationToken)
    {
        if (request.Week is > 1 or < 0) throw new UnknownException();


        //IS PERSONNEL ACTIVE

        var personnel = await personnelRepository.GetPersonnelAsync(request.PersonnelCode, cancellationToken);

        if (personnel == null)
        {
            logger.LogCritical(LogMessages.WrongUserToReserveMeal(request.PersonnelCode));

            throw new PersonnelNotExistException();
        }



        //CHECK PERSONNEL ACCESS    

        if (!await personnelRepository.PersonnelHasAccess(request.PersonnelCode)) throw new NotPermittedException();


        //GET FOOD

        var meal = await repository.GetMealAsync(request.FoodId, cancellationToken);

        if (meal == null)
        {
            logger.LogCritical(LogMessages.UserTriedReserveWrongMeal(request.PersonnelCode));

            throw new UnknownException();
        }



        //CAN RESERVE

        var reserveDate = PersianDateTime.GetPersianDateByWeekDay(meal.OrderDay, request.Week);

        var today = PersianDateTime.GetPersianDateByWeekDay((int)DateTime.Now.DayOfWeek);

        //IF IS EXPIRED
        //EX: foodDate = "1403/01/10"، today = "1403/01/11" => ANSWER: TRUE => return FALSE
        if (string.CompareOrdinal(reserveDate, today) <= 0) throw new ReserveOutOfDateException();


        var offDays = await offDaysRepository.GetOffDaysAsync(cancellationToken);

        //IF IS OFF
        if (offDays.Contains(reserveDate)) throw new ReserveIsOffException();



        //CHECK IF CANCELED BEFORE RESERVE

        var canceledReserve = await repository.GetCanceledReserveAsync(request.PersonnelCode, reserveDate, cancellationToken);

        if (canceledReserve != null)
        {
            //IF ALREADY RESERVED

            if (canceledReserve.CanceledReserve == false) return canceledReserve.Id;



            //RESERVE AGAIN

            canceledReserve.CanceledReserve = false;

            await repository.UpdateReserveAsync(canceledReserve, cancellationToken);

            return canceledReserve.Id;
        }



        //NEW RESERVE

        var priceForUser = (Convert.ToInt32(meal.Price) * (100 - personnel.Discount) / 100).ToString(CultureInfo.InvariantCulture);

        var reserve = new Reserve
        {
            MealId = request.FoodId,
            PersonnelCode = request.PersonnelCode,

            ReserveDate = reserveDate,
            DayName = meal.Day,
            PriceFood = meal.Price,
            PriceForUser = priceForUser,
            PercentOff = personnel.Discount.ToString(CultureInfo.InvariantCulture),
            TimeReserve = DateTime.Now.ToPersianDateTime(),

            PaymentOrder = "Etebari",
            WaitForValid = null,
            CanceledReserve = false,
            CausedCancel = null,
            TypeReserve = null,
            MACRegisteration = "TEST",
        };

        await repository.AddReserveAsync(reserve, cancellationToken);

        return reserve.Id;
    }
}
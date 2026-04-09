using Food_Reservation.Database;
using Food_Reservation.Entities;
using Food_Reservation.Shared;
using Food_Reservation.Tools;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Food_Reservation.Features.Restaurants;

public record ReserveFoodRequest(int FoodId, int PersonnelCode) : IRequest;

internal class ReserveFoodHandler(IReserveFoodAccess reserveFoodAccess, IReserveFoodRepository repository, IPersonnelRepository personnelRepository) : IRequestHandler<ReserveFoodRequest>
{
    public async Task Handle(ReserveFoodRequest request, CancellationToken cancellationToken)
    {
        //IS PERSONNEL ACTIVE

        var personnel = await personnelRepository.GetPersonnelAsync(request.PersonnelCode, cancellationToken);

        if (personnel == null) return /*TODO:Log FATAL*/;

        if (!personnel.Active) throw new PersonnelFiredException();



        //CHECK PERSONNEL ACCESS    

        if (!await personnelRepository.PersonnelHasAccess(request.PersonnelCode)) throw new NotPermittedException();


        //GET FOOD

        var food = await repository.GetFoodAsync(request.FoodId, cancellationToken);

        if (food == null) return /*TODO:Log FATAL*/;



        //CAN RESERVE

        var reserveDate = PersianDateTime.GetPersianDateByWeekDay(food.OrderDay);

        if (!await reserveFoodAccess.CanReserve(reserveDate, cancellationToken)) throw new ReserveAccessException();




        //CHECK IF CANCELED BEFORE RESERVE

        var canceledReserve = await repository.GetCanceledReserveAsync(request.PersonnelCode, reserveDate, cancellationToken);

        if (canceledReserve != null)
        {
            //IF ALREADY RESERVED

            if (canceledReserve.CanceledReserve == false) return;



            //RESERVE AGAIN

            canceledReserve.CanceledReserve = false;

            await repository.UpdateReserveAsync(canceledReserve,cancellationToken);

            return;
        }



        //NEW RESERVE

        var priceForUser = (Convert.ToInt32(food.Price) * (100 - personnel.Discount) / 100).ToString(CultureInfo.InvariantCulture);

        var reserve = new Reserve
        {
            FoodId = request.FoodId,
            PersonnelCode = request.PersonnelCode,

            ReserveDate = reserveDate,
            DayName = food.Day,
            PriceFood = food.Price,
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

        await repository.AddReserveAsync(reserve,cancellationToken);
    }
}




internal interface IReserveFoodAccess
{
    Task<bool> CanReserve(string reserveDate, CancellationToken cancellationToken);
}

internal class ReserveFoodAccess(IOffDaysRepository offDaysRepository) : IReserveFoodAccess
{
    public async Task<bool> CanReserve(string foodDate, CancellationToken cancellationToken)
    {
        var today = PersianDateTime.GetTodayPersianDate();

        //IF IS EXPIRED
        //EX: foodDate = "1403/01/10"، today = "1403/01/11" => ANSWER: TRUE => return FALSE
        if (string.CompareOrdinal(foodDate, today) <= 0) return false; /*TODO:Log FATAL*/;


        var offDays = await offDaysRepository.GetOffDaysAsync(cancellationToken);

        //IF IS OFF
        return !offDays.Contains(today);
    }
}





internal interface IReserveFoodRepository
{
    Task<Food?> GetFoodAsync(int id, CancellationToken cancellationToken);
    Task<Reserve?> GetCanceledReserveAsync(int personnelCode, string reserveDate, CancellationToken cancellationToken);



    Task AddReserveAsync(Reserve reserve, CancellationToken cancellationToken);
    Task UpdateReserveAsync(Reserve reserve, CancellationToken cancellationToken);
}

internal class ReserveFoodRepository(AppDbContext context) : IReserveFoodRepository
{

    public async Task<Food?> GetFoodAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Foods.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }



    public async Task<Reserve?> GetCanceledReserveAsync(int personnelCode, string reserveDate, CancellationToken cancellationToken)
    {
        return await context.Reserve
            .FirstOrDefaultAsync(r => r.PersonnelCode == personnelCode && r.ReserveDate == reserveDate,
                cancellationToken);
    }



    public async Task AddReserveAsync(Reserve reserve,CancellationToken cancellationToken)
    {
        await context.Reserve.AddAsync(reserve,cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateReserveAsync(Reserve reserve, CancellationToken cancellationToken)
    {
        context.Reserve.Update(reserve);

        await context.SaveChangesAsync(cancellationToken);
    }
}





internal class ReserveAccessException : Exception;


public static class ReserveFoodRegister
{
    public static void RegisterReserveFood(this IServiceCollection services)
    {
        services.AddScoped<IReserveFoodAccess, ReserveFoodAccess>();

        services.AddScoped<IReserveFoodRepository, ReserveFoodRepository>();
    }
}
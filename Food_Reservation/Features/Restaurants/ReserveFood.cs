using System.Globalization;
using Food_Reservation.Database;
using Food_Reservation.Entities;
using Food_Reservation.Tools;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Food_Reservation.Features.Restaurants;

public record ReserveFoodRequest(int FoodId, int PersonnelCode) : IRequest;

internal class ReserveFoodHandler(AppDbContext context, IReserveAccess reserveAccess) : IRequestHandler<ReserveFoodRequest>
{
    public async Task Handle(ReserveFoodRequest request, CancellationToken cancellationToken)
    {
        if (!await reserveAccess.HasAccess(request.PersonnelCode)) throw new PersonnelNotAccessException();

        wrng query
        var personnel = await context.Personnel.AsNoTracking()
            .FirstOrDefaultAsync(p => p.PersonnelCode == request.PersonnelCode, cancellationToken);

        if (personnel == null) return;


        var food = await context.Foods.AsNoTracking().FirstOrDefaultAsync(f => f.Id == request.FoodId, cancellationToken);

        if (food == null) return;

        var canceledReserve = await context.Reserve
            .FirstOrDefaultAsync(r => r.FoodId == request.FoodId && r.PersonnelCode == request.PersonnelCode,
                cancellationToken);

        if (canceledReserve != null)
        {
            canceledReserve.CanceledReserve = false;

            context.Reserve.Update(canceledReserve);

            await context.SaveChangesAsync(cancellationToken);

            return;
        }

        var today = DateTime.Now/*.AddDays(1)*/.DayOfWeek + 2;
        var daysToSaturday = today == DayOfWeek.Sunday ? 6 : (int)today;
        var startOfWeekDate = DateTime.Now.AddDays(-daysToSaturday).Date;

        var weekDays = new List<DateTime>
        {
            startOfWeekDate.AddDays(0),
            startOfWeekDate.AddDays(1),
            startOfWeekDate.AddDays(2),
            startOfWeekDate.AddDays(3),
            startOfWeekDate.AddDays(4),
            startOfWeekDate.AddDays(5),
            startOfWeekDate.AddDays(6)
        };

        var reserve = new Reserve()
        {
            FoodId = request.FoodId,
            PersonnelCode = request.PersonnelCode,

            ReserveDate = weekDays[food.OrderDay].ToPersianDate(),
            DayName = food.Day,
            PriceFood = food.Price,
            PriceForUser = (Convert.ToInt32(food.Price) * (100 - personnel.Discount) / 100).ToString(CultureInfo.InvariantCulture),
            PercentOff = personnel.Discount.ToString(CultureInfo.InvariantCulture),
            TimeReserve = DateTime.Now.ToPersianDateTime(),

            PaymentOrder = "Etebari",
            WaitForValid = null,
            CanceledReserve = false,
            CausedCancel = null,
            TypeReserve = null,
            MACRegisteration = "TEST",
        };

        await context.Reserve.AddAsync(reserve, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

    }
}

internal interface IReserveAccess
{
    Task<bool> HasAccess(int personnelCode);
}

internal class ReserveAccess : IReserveAccess
{
    public async Task<bool> HasAccess(int personnelCode)
    {


        return true;
    }
}

internal class PersonnelNotAccessException: Exception;
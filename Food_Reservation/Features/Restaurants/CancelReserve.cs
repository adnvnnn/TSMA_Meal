using Food_Reservation.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Food_Reservation.Features.Restaurants;

public record CancelReserveRequest(int FoodId,int PersonnelCode) : IRequest;

public class CancelReserveHandler(AppDbContext context) : IRequestHandler<CancelReserveRequest>
{
    public async Task Handle(CancelReserveRequest request, CancellationToken cancellationToken)
    {

        wrng query
        var reserve = await context.Reserve
            .FirstOrDefaultAsync(r => r.FoodId == request.FoodId && r.PersonnelCode == request.PersonnelCode,
                cancellationToken);


        if (reserve == null) return;

        reserve.CanceledReserve = true;

        context.Reserve.Update(reserve);

        await context.SaveChangesAsync(cancellationToken);
    }
}
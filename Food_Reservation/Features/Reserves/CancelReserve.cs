using MediatR;
using Microsoft.EntityFrameworkCore;
using TSMA_Meal.Database;
using TSMA_Meal.Entities;
using TSMA_Meal.Features.Account;
using TSMA_Meal.Shared;
using TSMA_Meal.Tools;

namespace TSMA_Meal.Features.Reserves;

public record CancelReserveRequest(int ReserveId,int PersonnelCode) : IRequest;

public class CancelReserveHandler(ICancelReserveAccess cancelReserveAccess,ICancelReserveRepository repository,IPersonnelRepository personnelRepository) : IRequestHandler<CancelReserveRequest>
{
    public async Task Handle(CancelReserveRequest request, CancellationToken cancellationToken)
    {
        if (request.ReserveId == 0) return;
        

        //IS PERSONNEL ACTIVE

        var personnel = await personnelRepository.GetPersonnelAsync(request.PersonnelCode, cancellationToken);

        if (personnel == null) throw new PersonnelNotExistException() /*TODO:Log FATAL*/;



        //CHECK PERSONNEL ACCESS    

        if (!await personnelRepository.PersonnelHasAccess(request.PersonnelCode)) throw new NotPermittedException();



        //GET RESERVATION

        var reserve = await repository.GetReserveAsync(request.ReserveId,cancellationToken);

        if (reserve == null) return /*TODO:Log FATAL*/;

        if (reserve.PersonnelCode != request.PersonnelCode) return /*TODO:Log FATAL*/;



        //CAN RESERVE

        if (!cancelReserveAccess.CanCancel(reserve.ReserveDate)) return /*TODO:Log FATAL*/;


        //IF CANCELED BEFORE 

        if (reserve.CanceledReserve) return;



        //CANCEL RESERVE 

        reserve.CanceledReserve = true;

        await repository.UpdateReserveAsync(reserve,cancellationToken);
    }
}

public interface ICancelReserveAccess
{
    bool CanCancel(string foodDate);
}
internal class CancelReserveAccess : ICancelReserveAccess
{
    public bool CanCancel(string foodDate)
    {
        var today = PersianDateTime.GetPersianDateByWeekDay((int)DateTime.Now.DayOfWeek);

        //IF IS EXPIRED
        //EX: foodDate = "1403/01/10"، today = "1403/01/11" => ANSWER: TRUE => return FALSE
        if (string.CompareOrdinal(foodDate, today) <= 0) return false; /*TODO:Log FATAL*/;

        return true;
    }
}


public interface ICancelReserveRepository
{
    Task<Reserve?> GetReserveAsync(int id,CancellationToken cancellationToken);

    Task UpdateReserveAsync(Reserve reserve, CancellationToken cancellationToken);

}

internal class CancelReserveRepository(AppDbContext context) : ICancelReserveRepository
{
    public async Task<Reserve?> GetReserveAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Reserve
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task UpdateReserveAsync(Reserve reserve, CancellationToken cancellationToken)
    {
        context.Reserve.Update(reserve);

        await context.SaveChangesAsync(cancellationToken);
    }
}




public static class CancelReserveRegister
{
    public static void RegisterCancelReserve(this IServiceCollection services)
    {
        services.AddScoped<ICancelReserveAccess, CancelReserveAccess>();

        services.AddScoped<ICancelReserveRepository, CancelReserveRepository>();
    }
}
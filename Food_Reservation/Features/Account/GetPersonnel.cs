using MediatR;
using TSMA_Meal.Shared;

namespace TSMA_Meal.Features.Account;

public record PersonnelByPhoneQuery(string Phone) : IRequest<PersonnelViewModel>;

public class PersonnelByPhoneHandler(IPersonnelRepository repository, IHttpContextAccessor httpContextAccessor) : IRequestHandler<PersonnelByPhoneQuery, PersonnelViewModel>
{
    public async Task<PersonnelViewModel> Handle(PersonnelByPhoneQuery request, CancellationToken cancellationToken)
    {
        var personnel = await repository.GetPersonnelByPhoneAsync(request.Phone,cancellationToken);

        if (personnel == null) throw new NotPermittedException();


        return new PersonnelViewModel()
        {
            FullName = personnel.Name + " " + personnel.LastName,
            PersonnelCode = personnel.PersonnelCode
        };
    }
}

public class PersonnelViewModel
{
    public string FullName { get; set; }
    public int PersonnelCode { get; set; }
}
using MediatR;

namespace TSMA.Meal.Web.Features.Meals.CancelReserve;

public record CancelReserveRequest(int ReserveId, int PersonnelCode) : IRequest;

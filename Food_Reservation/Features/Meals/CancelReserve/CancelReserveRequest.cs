using MediatR;

namespace TSMA_Meal.Features.Meals.CancelReserve;

public record CancelReserveRequest(int ReserveId, int PersonnelCode) : IRequest;

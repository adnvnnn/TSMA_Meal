using TSMA_Meal.Shared.Exceptions;

namespace TSMA_Meal.Features.Account.Login;

public class InvalidCodeException() : BaseException("کد وارد شده صحیح نمیباشد.");
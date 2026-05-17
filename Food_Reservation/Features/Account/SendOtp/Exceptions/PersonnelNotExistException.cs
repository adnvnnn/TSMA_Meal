using TSMA_Meal.Shared.Exceptions;

namespace TSMA_Meal.Features.Account.SendOtp.Exceptions;

internal class PersonnelNotExistException() : BaseException("شما دسترسی ندارید. فقط پرسنل مجازند.");

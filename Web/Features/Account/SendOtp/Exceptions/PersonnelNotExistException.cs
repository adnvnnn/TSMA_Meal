using TSMA.Meal.Web.Shared.Exceptions;

namespace TSMA.Meal.Web.Features.Account.SendOtp.Exceptions;

internal class PersonnelNotExistException() : BaseException("شما دسترسی ندارید. فقط پرسنل مجازند.");

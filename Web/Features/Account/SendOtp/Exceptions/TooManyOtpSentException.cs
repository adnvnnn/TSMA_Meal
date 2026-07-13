using TSMA.Meal.Web.Shared.Exceptions;

namespace TSMA.Meal.Web.Features.Account.SendOtp.Exceptions;

internal class TooManyOtpSentException() : BaseException("تعداد تلاش‌ها بیش از حد. ۲۴ ساعت دیگر امتحان کنید.");

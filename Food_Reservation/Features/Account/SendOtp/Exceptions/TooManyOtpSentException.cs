using TSMA_Meal.Shared.Exceptions;

namespace TSMA_Meal.Features.Account.SendOtp.Exceptions;

internal class TooManyOtpSentException() : BaseException("تعداد تلاش‌ها بیش از حد. ۲۴ ساعت دیگر امتحان کنید.");

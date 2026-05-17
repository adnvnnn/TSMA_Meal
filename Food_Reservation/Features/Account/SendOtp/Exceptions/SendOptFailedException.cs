using TSMA_Meal.Shared.Exceptions;

namespace TSMA_Meal.Features.Account.SendOtp.Exceptions;

internal class SendOptFailedException() : BaseException("خطا در ارسال کد. لطفاً مجدد تلاش کنید.");

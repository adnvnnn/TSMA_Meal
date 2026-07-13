using TSMA.Meal.Web.Shared.Exceptions;

namespace TSMA.Meal.Web.Features.Account.SendOtp.Exceptions;

internal class TrySendOtpAfterMinutesException(int minutes) : BaseException($"لطفا بعد از {minutes} دقیقه تلاش کنید.");

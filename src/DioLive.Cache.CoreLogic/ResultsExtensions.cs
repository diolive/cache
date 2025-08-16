using DioLive.Cache.Common;

using DioRed.Common;

namespace DioLive.Cache.CoreLogic;
public static class ResultsExtensions
{
    public static Result<T> NotFoundIfNull<T>(this Result<T?> result)
        where T : class
    {
        if (result.IsSuccess && result.Value is null)
        {
            return Result.Fail(new Error(ErrorCodes.NotFound));
        }

        return result!;
    }

    public static Result<T> NotFoundIfNull<T>(this Result<T?> result)
        where T : struct
    {
        if (result.IsSuccess)
        {
            if (result.Value is null)
            {
                return Result.Fail(new Error(ErrorCodes.NotFound));
            }
            else
            {
                return Result.Success(result.Value.Value);
            }
        }

        return Result.Fail(result.Errors);
    }
}
using System.Diagnostics.CodeAnalysis;
using System.Security;

using DioLive.Cache.Common;

using DioRed.Common;

namespace DioLive.Cache.CoreLogic.Exceptions;

public class ValidationException : SecurityException
{
    public ValidationException()
    {
    }

    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public static void RaiseIfNeeded(Result result)
    {
        if (result.IsSuccess) return;

        switch (result.Errors[0].Code)
        {
            case ErrorCodes.NotFound:
                throw new NotFoundException("Not found");

            case ErrorCodes.Forbidden:
                RaiseForbidden();
                break;

            case ErrorCodes.UnexpectedError:
                RaiseUnexpectedError();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
        ;
    }

    [DoesNotReturn]
    public static void RaiseForbidden()
    {
        throw new ValidationException("User has no permissions");
    }

    [DoesNotReturn]
    public static void RaiseUnexpectedError()
    {
        throw new InvalidOperationException("Unexpected error occured");
    }
}
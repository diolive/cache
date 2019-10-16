namespace DioLive.Cache.Common
{
	public static class ResultExtensions
	{
		public static Result<T> NullMeansNotFound<T>(this Result<T?> result)
			where T : struct
		{
			return result.IsSuccess
				? result.Data.HasValue
					? new Result<T>(result.Data.Value)
					: new Result<T>(ResultStatus.NotFound, result.ErrorMessage)
				: new Result<T>(result.Status, result.ErrorMessage);
		}

		public static Result<T> NullMeansNotFound<T>(this Result<T?> result)
			where T : class
		{
			return result.IsSuccess
				? result.Data != null
					? new Result<T>(result.Data)
					: new Result<T>(ResultStatus.NotFound, result.ErrorMessage)
				: new Result<T>(result.Status, result.ErrorMessage);
		}
	}
}
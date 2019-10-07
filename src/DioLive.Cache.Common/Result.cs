using System;

namespace DioLive.Cache.Common
{
	public class Result
	{
		public Result(ResultStatus status, string? errorMessage = null)
		{
			Status = status;
			ErrorMessage = errorMessage;
		}

		public ResultStatus Status { get; }

		public string? ErrorMessage { get; }

		public bool IsSuccess => Status == ResultStatus.Success;

		public static Result<T> Ok<T>(T data)
		{
			return new Result<T>(data);
		}

		public static implicit operator Result(ResultStatus status)
		{
			return new Result(status);
		}
	}

	public class Result<TData> : Result
	{
		private readonly TData _data = default!;
		private readonly bool _hasData;

		public Result(TData data)
			: base(ResultStatus.Success)
		{
			_data = data;
			_hasData = true;
		}

		public TData Data => _hasData ? _data : throw new InvalidOperationException("No data was returned");


		public static implicit operator Result<TData>(ResultStatus status)
		{
			return new Result<TData>(status);
		}

		public Result(ResultStatus status, string? errorMessage = null)
			: base(status, errorMessage)
		{
			_hasData = false;
		}

		public Result(Result result)
			: base(result.Status, result.ErrorMessage)
		{
			_hasData = false;
		}
	}
}
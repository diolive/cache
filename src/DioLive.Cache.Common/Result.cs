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

		public Result<TResult> Then<TResult>(Func<Result<TResult>> next)
		{
			return IsSuccess
				? next()
				: new Result<TResult>(this);
		}

		public Result Then(Func<Result> next)
		{
			return IsSuccess
				? next()
				: this;
		}

		public void Then(Action next)
		{
			if (IsSuccess)
			{
				next();
			}
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

		public TData Data => _hasData ? _data : throw new InvalidOperationException("No data was returned");

		public Result<TResult> Then<TResult>(Func<TData, Result<TResult>> next)
		{
			return IsSuccess
				? next(Data)
				: new Result<TResult>(this);
		}

		public Result Then(Func<TData, Result> next)
		{
			return IsSuccess
				? next(Data)
				: this;
		}

		public void Then(Action<TData> next)
		{
			if (IsSuccess)
			{
				next(Data);
			}
		}

		public static implicit operator Result<TData>(ResultStatus status)
		{
			return new Result<TData>(status);
		}
	}
}
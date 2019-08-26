using System;

namespace DioLive.Cache.Storage.Contracts
{
	public interface ICurrentContext
	{
		string UICulture { get; }
		Guid? BudgetId { get; }
		string UserId { get; }
	}
}
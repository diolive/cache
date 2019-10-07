using System;

namespace DioLive.Cache.Common
{
	public interface ICurrentContext
	{
		string Culture { get; }
		Guid? BudgetId { get; set; }
		string UserId { get; }
	}
}
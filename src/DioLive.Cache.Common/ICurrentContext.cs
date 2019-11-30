using System;

using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.Common
{
	public interface ICurrentContext
	{
		string Culture { get; }
		string UserId { get; }
		BudgetSlim? Budget { get; set; }
		Guid? BudgetId { get; }
	}
}
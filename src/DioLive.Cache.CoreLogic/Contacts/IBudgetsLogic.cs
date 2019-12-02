using System;
using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.CoreLogic.Contacts
{
	public interface IBudgetsLogic
	{
		Result<Guid> Create(string budgetName, string currencyId);
		Result Delete();
		Result<string> GetName();
		Result<(string name, string authorName)> GetNameAndAuthor();
		Result<BudgetSlim> Open(Guid budgetId);
		Result Rename(string newBudgetName);
		Result Share(string targetUserName, ShareAccess targetAccess);
		Result<IReadOnlyCollection<ShareItem>> GetShares();
		Result<IReadOnlyCollection<Budget>> GetAllAvailable();
		Result<string> GetCurrencySign();
	}
}
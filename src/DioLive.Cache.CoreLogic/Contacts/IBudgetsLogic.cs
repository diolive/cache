﻿using System;
using System.Collections.Generic;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Entities;

namespace DioLive.Cache.CoreLogic.Contacts
{
	public interface IBudgetsLogic
	{
		Result<Guid> Create(string budgetName);
		Result Delete();
		Result<string> GetName();
		Result<(string name, string authorName)> GetNameAndAuthor();
		Result Open(Guid budgetId);
		Result Rename(string newBudgetName);
		Result Share(string targetUserId, ShareAccess targetAccess);
		Result<IReadOnlyCollection<ShareItem>> GetShares();
		Result<IReadOnlyCollection<Budget>> GetAllAvailable();
	}
}
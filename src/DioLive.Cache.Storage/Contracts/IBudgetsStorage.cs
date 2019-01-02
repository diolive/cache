using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Models;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IBudgetsStorage
	{
		Task<Budget> GetByIdAsync(Guid id);
		Task<Budget> GetWithSharesAsync(Guid id);
		Task<Budget> GetForBudgetSharingComponentAsync(Guid id);
		Task<List<Budget>> GetForUserBudgetsComponentAsync(string userId);
		Task AddAsync(Budget budget);
		Task<Result> RenameAsync(Guid id, string userId, string name);
		Task<(Result, Budget)> GetForRemoveAsync(Guid id, string userId);
		Task<Result> RemoveAsync(Guid id, string userId);
		Task<Result> ShareAsync(Guid id, string authorId, string targetUserId, ShareAccess access);
		Task<(Result, Budget)> OpenAsync(Guid id, string userId);
		Task MigrateAsync(Guid id);
	}
}
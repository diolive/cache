using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IBudgetsStorage
	{
		Task<Budget> GetDetailsAsync(Guid id);
		Task<(Result, Budget)> GetAsync(Guid id, ShareAccess shareAccess);
		Task<IReadOnlyCollection<Budget>> GetForUserBudgetsComponentAsync();
		Task<Guid> AddAsync(string name);
		Task<Result> RenameAsync(Guid id, string name);
		Task<Result> RemoveAsync(Guid id);
		Task<Result> ShareAsync(Guid id, string userId, ShareAccess access);
		Task<(Result, Budget)> OpenAsync(Guid id);
		Task<Result> MigrateAsync(Guid id);
		Task<IReadOnlyCollection<Share>> GetSharesAsync(Guid budgetId);
	}
}
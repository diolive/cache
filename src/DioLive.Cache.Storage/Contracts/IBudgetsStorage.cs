using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Storage.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IBudgetsStorage
	{
		Task<Budget> GetAsync(Guid id);
		Task<IReadOnlyCollection<Budget>> GetAllAvailableAsync();
		Task<Guid> AddAsync(string name);
		Task RenameAsync(Guid id, string name);
		Task DeleteAsync(Guid id);
		Task ShareAsync(Guid id, string userId, ShareAccess access);
		Task<IReadOnlyCollection<Share>> GetSharesAsync(Guid budgetId);
		Task<byte> GetVersionAsync(Guid budgetId);
		Task SetVersionAsync(Guid id, byte version);
	}
}
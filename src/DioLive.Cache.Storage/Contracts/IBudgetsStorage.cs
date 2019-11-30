using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DioLive.Cache.Common.Entities;

namespace DioLive.Cache.Storage.Contracts
{
	public interface IBudgetsStorage
	{
		Task<Budget> GetAsync(Guid id);
		Task<IReadOnlyCollection<Budget>> GetAllAvailableAsync();
		Task<Guid> AddAsync(string name, string currencyId);
		Task RenameAsync(Guid id, string name);
		Task DeleteAsync(Guid id);
		Task ShareAsync(Guid id, string userId, ShareAccess access);
		Task<IReadOnlyCollection<ShareItem>> GetSharesAsync(Guid budgetId);
		Task<byte> GetVersionAsync(Guid budgetId);
		Task SetVersionAsync(Guid id, byte version);
		Task<string> GetCurrencyAsync(Guid id);
	}
}
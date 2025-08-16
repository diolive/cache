using Dapper;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.Storage.SqlServer;

public class CategoriesStorage(
    IConnectionInfo connectionInfo,
    ICurrentContext currentContext
) : StorageBase(connectionInfo, currentContext), ICategoriesStorage
{
    public async Task<Category?> GetAsync(int id)
    {
        return await Connection.QuerySingleOrDefaultAsync<Category>(
            Queries.Categories.Select,
            new
            {
                Id = id
            }
        );
    }

    public async Task<IReadOnlyCollection<Category>> GetAllAsync(Guid budgetId)
    {
        return
        [
            .. await Connection.QueryAsync<Category>(
                Queries.Categories.SelectAll,
                new
                {
                    BudgetId = budgetId
                }
            )
        ];
    }

    public async Task<int?> GetMostPopularIdAsync(Guid budgetId)
    {
        return await Connection.QuerySingleOrDefaultAsync<int?>(
            Queries.Categories.SelectMostPopularId,
            new
            {
                BudgetId = budgetId
            }
        );
    }

    public async Task InitializeCategoriesAsync(Guid budgetId)
    {
        await CloneCommonCategories(CurrentUserId, budgetId);
    }

    public async Task<int> AddAsync(string name, Guid budgetId)
    {
        var category = new Category
        {
            Name = name,
            BudgetId = budgetId,
            OwnerId = CurrentUserId,
            Color = GetRandomColor()
        };

        return await Connection.ExecuteScalarAsync<int>(Queries.Categories.Insert, category);

        static int GetRandomColor()
        {
            return Random.Shared.Next(1 << 24);
        }
    }

    public async Task UpdateAsync(int id, int? parentId, string name, string color)
    {
        await Connection.ExecuteAsync(
            Queries.Categories.Update,
            new
            {
                Id = id,
                Name = name,
                ParentId = parentId,
                Color = color
            }
        );
    }

    public async Task DeleteAsync(int id)
    {
        await Connection.ExecuteAsync(
            Queries.Categories.Delete,
            new
            {
                Id = id
            }
        );
    }

    public async Task<int?> GetLatestAsync(Guid budgetId, string purchase)
    {
        return await Connection.QuerySingleOrDefaultAsync<int?>(
            Queries.Categories.GetLatest,
            new
            {
                BudgetId = budgetId,
                Name = purchase
            }
        );
    }

    public async Task<CategoryWithTotals[]> GetWithTotalsAsync(Guid budgetId, int days = 0)
    {
        IReadOnlyCollection<Category> categories = await GetAllAsync(budgetId);
        IEnumerable<Category> rootCategories = categories.Where(c => !c.ParentId.HasValue);

        CategoryWithTotals[] categoriesWithTotal =
        [
            .. await Connection.QueryAsync<CategoryWithTotals>(
                Queries.Categories.GetWithTotals,
                new
                {
                    BudgetId = budgetId,
                    Days = days
                }
            )
        ];

        foreach (CategoryWithTotals categoryWithTotals in categoriesWithTotal)
        {
            categoryWithTotals.Children =
            [
                ..categories
                    .Where(c => c.ParentId == categoryWithTotals.Id)
                    .Select(c => categoriesWithTotal.SingleOrDefault(ct => ct.Id == c.Id))
                    .Where(c => c != null)!
            ];
        }

        return
        [
            .. rootCategories
                .Select(rc => categoriesWithTotal.SingleOrDefault(ct => ct.Id == rc.Id))
                .Where(c => c != null)!
        ];
    }

    public async Task CloneCommonCategories(string userId, Guid budgetId)
    {
        Category[] commonCategories =
        [
            .. await Connection.QueryAsync<Category>(Queries.Categories.SelectCommon)
        ];

        Category[] rootCategories =
        [
            .. commonCategories.Where(c => !c.ParentId.HasValue)
        ];

        foreach (Category rootCategory in rootCategories)
        {
            await CloneCategory(rootCategory);
        }

        return;

        async Task CloneCategory(Category category)
        {
            int oldId = category.Id;

            category.OwnerId = userId;
            category.BudgetId = budgetId;

            int newId = await Connection.ExecuteScalarAsync<int>(Queries.Categories.Clone, category);

            Category[] children =
            [
                .. commonCategories
                    .Where(c => c.ParentId == oldId)
            ];

            foreach (Category child in children)
            {
                child.ParentId = newId;
                await CloneCategory(child);
            }

            await Connection.ExecuteAsync(
                Queries.Purchases.UpdateCategory,
                new
                {
                    BudgetId = budgetId,
                    OldCategoryId = oldId,
                    NewCategoryId = newId
                }
            );
        }
    }
}
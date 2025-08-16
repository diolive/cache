using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.CoreLogic.Entities;
using DioLive.Cache.Storage;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs.Charts;

[Authenticated]
[HasAnyRights]
public class GetJob(int days, int depth, int step) : Job<ChartData>
{
    protected override async Task<ChartData> ExecuteAsync()
    {
        IStorageCollection storageCollection = Settings.StorageCollection;

        string currentCulture = CurrentContext.GetCulture();

        int daysCount = (days - 1) * step + depth;
        DateTime today = DateTime.Today;
        DateTime tomorrow = today.AddDays(1);
        DateTime minDate = tomorrow.AddDays(-daysCount);

        IReadOnlyCollection<Category> categories = await storageCollection.Categories.GetAllAsync(CurrentBudget);
        var allCategories = new Hierarchy<Category, int>(categories, c => c.Id, c => c.ParentId);

        ILookup<(int CategoryId, DateTime Date), Purchase> purchases = (await storageCollection.Purchases.GetForStatAsync(CurrentBudget, minDate, tomorrow))
            .ToLookup(p => (p.CategoryId, p.Date));

        Dictionary<int, Hierarchy<Category, int>.Node> roots = purchases
            .Select(p => p.Key.CategoryId)
            .Distinct()
            .ToDictionary(c => c, c => allCategories[c].Root);

        Category[] rootCategories = roots.Values.Select(r => r.Value).ToArray();
        DateTime[] dates = Enumerable.Range(0, daysCount).Select(n => minDate.AddDays(n)).ToArray();
        var statData = new decimal[days][];

        for (var dy = 0; dy < statData.Length; dy++)
        {
            statData[dy] = new decimal[rootCategories.Length];
            DateTime dateFrom = dates[dy * step];
            DateTime dateTo = dateFrom.AddDays(depth);

            for (var ct = 0; ct < rootCategories.Length; ct++)
            {
                Category category = rootCategories[ct];
                statData[dy][ct] = purchases
                    .Where(p => roots[p.Key.CategoryId].Value == category && p.Key.Date >= dateFrom &&
                                p.Key.Date < dateTo)
                    .SelectMany(p => p)
                    .Sum(p => p.Cost);
            }
        }

        return new ChartData
        {
            Columns =
            [
                .. rootCategories.Select(cat => new ChartDataColumn
                {
                    Name = cat.Name,
                    Color = cat.Color.ToString("X6")
                })
            ],
            Data =
            [
                .. statData.Select((stat, index) => new ChartDataItem
                {
                    Date = dates[index * step].ToString(Constants.DateFormat),
                    Values = stat
                })
            ]
        };
    }
}
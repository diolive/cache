using System;

using DioLive.BlackMint.Entities;

namespace DioLive.BlackMint.Persistence.SQLite
{
    internal static class SqlHelper
    {
        public static string ToSql(this SelectOrder order, string field)
        {
            switch (order)
            {
                case SelectOrder.Ascending:
                    return field + " ASC";

                case SelectOrder.Descending:
                    return field + " DESC";

                default:
                    throw new ArgumentException($"Unsupported value: {order}", nameof(order));
            }
        }
    }
}
using System;

using Microsoft.AspNetCore.Mvc;

namespace DioLive.Cache.WebUI.Models
{
    public class LoadResult<TData>
    {
        private LoadResult(TData data, Func<Controller, IActionResult> getActionResult, bool success)
        {
            Data = data;
            GetActionResult = getActionResult;
            Success = success;
        }

        public TData Data { get; }

        public Func<Controller, IActionResult> GetActionResult { get; }

        public bool Success { get; }

        public static LoadResult<TData> Complete(TData data)
        {
            return new LoadResult<TData>(data, c => c.Ok(), true);
        }

        public static LoadResult<TData> Fail(Func<Controller, IActionResult> getActionResult)
        {
            return new LoadResult<TData>(default(TData), getActionResult, false);
        }
    }
}
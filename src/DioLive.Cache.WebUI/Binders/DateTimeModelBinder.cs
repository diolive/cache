using System;
using System.Threading.Tasks;

using DioLive.Cache.Storage;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DioLive.Cache.WebUI.Binders
{
	public class DateTimeModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			DateTime date = DateTime.ParseExact(result.FirstValue, Constants.DateFormat, null);

			bindingContext.Result = ModelBindingResult.Success(date);

			return Task.CompletedTask;
		}
	}

	public class DateTimeModelBinderProvider : IModelBinderProvider
	{
		private static readonly Lazy<DateTimeModelBinder> _dateTimeModelBinder;

		static DateTimeModelBinderProvider()
		{
			_dateTimeModelBinder = new Lazy<DateTimeModelBinder>(() => new DateTimeModelBinder());
		}

		public IModelBinder? GetBinder(ModelBinderProviderContext context)
		{
			if (context is null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			return context.Metadata.ModelType == typeof(DateTime)
				? _dateTimeModelBinder.Value
				: null;
		}
	}
}
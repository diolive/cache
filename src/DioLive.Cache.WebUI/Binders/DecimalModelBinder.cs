using System;
using System.Globalization;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DioLive.Cache.WebUI.Binders
{
	public class DecimalModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			string valueString = result.FirstValue.Replace(',', '.');
			decimal value = decimal.Parse(valueString, CultureInfo.InvariantCulture);

			bindingContext.Result = ModelBindingResult.Success(value);

			return Task.CompletedTask;
		}
	}

	public class DecimalModelBinderProvider : IModelBinderProvider
	{
		private static readonly Lazy<DecimalModelBinder> _decimalModelBinder;

		static DecimalModelBinderProvider()
		{
			_decimalModelBinder = new Lazy<DecimalModelBinder>(() => new DecimalModelBinder());
		}

		public IModelBinder? GetBinder(ModelBinderProviderContext context)
		{
			if (context is null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			return context.Metadata.ModelType == typeof(decimal)
				? _decimalModelBinder.Value
				: null;
		}
	}
}
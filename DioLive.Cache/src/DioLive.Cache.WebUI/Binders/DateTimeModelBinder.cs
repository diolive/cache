using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DioLive.Cache.WebUI.Binders
{
    public class DateTimeModelBinder : IModelBinder
    {
        public const string DateFormat = "yyyy-MM-dd";
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm";

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            DateTime date = DateTime.ParseExact(result.ConvertTo<string>(), DateTimeModelBinder.DateFormat, null);

            bindingContext.Result = ModelBindingResult.Success(date);

            return Task.CompletedTask;
        }
    }

    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType != typeof(DateTime))
            {
                return null;
            }

            return new DateTimeModelBinder();
        }
    }
}
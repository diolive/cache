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
            DateTime date = DateTime.ParseExact(result.FirstValue, DateFormat, null);

            bindingContext.Result = ModelBindingResult.Success(date);

            return Task.CompletedTask;
        }
    }

    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        private static readonly DateTimeModelBinder DateTimeModelBinder;
        private static readonly ArgumentNullException ContextNullException;

        static DateTimeModelBinderProvider()
        {
            DateTimeModelBinder = new DateTimeModelBinder();
            ContextNullException = new ArgumentNullException("context");
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            context = context ?? throw ContextNullException;

            return context.Metadata.ModelType == typeof(DateTime)
                ? DateTimeModelBinder
                : null;
        }
    }
}
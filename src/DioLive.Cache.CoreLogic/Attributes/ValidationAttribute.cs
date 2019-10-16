using System;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Exceptions;

namespace DioLive.Cache.CoreLogic.Attributes
{
	public abstract class ValidationAttribute : Attribute
	{
		protected Guid CurrentBudget(ICurrentContext currentContext)
		{
			return currentContext.BudgetId ?? throw new NotFoundException("Budget not found");
		}

		public abstract void Validate(ICurrentContext currentContext);
	}
}
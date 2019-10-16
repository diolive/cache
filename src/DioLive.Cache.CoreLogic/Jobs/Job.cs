using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.Common.Entities;
using DioLive.Cache.CoreLogic.Attributes;
using DioLive.Cache.CoreLogic.Exceptions;
using DioLive.Cache.Storage.Contracts;

namespace DioLive.Cache.CoreLogic.Jobs
{
	public abstract class JobBase
	{
		private ICurrentContext? _currentContext;
		private JobSettings? _settings;

		public JobSettings Settings
		{
			get => _settings ?? JobSettings.Default;
			set => _settings = value;
		}

		protected ICurrentContext CurrentContext
		{
			get => _currentContext ?? throw new InvalidOperationException("Current context was not provided by Validate() method");
			set => _currentContext = value;
		}

		protected Guid CurrentBudget => CurrentContext.BudgetId ?? throw new InvalidOperationException("Budget was not opened");

		protected void AssertUserIsAuthenticated()
		{
			if (string.IsNullOrEmpty(CurrentContext.UserId))
			{
				throw new ValidationException("User should be authenticated");
			}
		}

		protected void AssertUserHasAccessForBudget(Guid budgetId, ShareAccess requiredAccess)
		{
			IPermissionsValidator permissionsValidator = Settings.PermissionsValidator;
			ResultStatus result = permissionsValidator.CheckUserRightsForBudget(budgetId, CurrentContext.UserId, requiredAccess);

			ValidationException.RaiseIfNeeded(result);
		}

		protected void AssertCategoryIsInCurrentBudget(int categoryId)
		{
			Category? category = Settings.StorageCollection.Categories.GetAsync(categoryId).GetAwaiter().GetResult();

			if (category is null)
			{
				ValidationException.RaiseIfNeeded(ResultStatus.NotFound);
			}

			if (category?.BudgetId != CurrentBudget)
			{
				ValidationException.RaiseIfNeeded(ResultStatus.Forbidden);
			}
		}

		protected void AssertPurchaseIsInCurrentBudget(Guid purchaseId)
		{
			Purchase? purchase = Settings.StorageCollection.Purchases.GetAsync(purchaseId).GetAwaiter().GetResult();

			if (purchase is null)
			{
				ValidationException.RaiseIfNeeded(ResultStatus.NotFound);
			}

			if (purchase?.BudgetId != CurrentBudget)
			{
				ValidationException.RaiseIfNeeded(ResultStatus.Forbidden);
			}
		}

		protected void ValidateInternal(ICurrentContext currentContext)
		{
			CurrentContext = currentContext;

			if (Settings.UseAttributeValidation)
			{
				ProcessValidationAttributes();
			}

			CustomValidation();
		}

		protected virtual void CustomValidation()
		{
		}

		private void ProcessValidationAttributes()
		{
			IEnumerable<ValidationAttribute> attributes = GetType().GetCustomAttributes<ValidationAttribute>();
			foreach (ValidationAttribute attribute in attributes)
			{
				if (attribute is HasRightsAttribute x)
				{
					x.PermissionsValidator = Settings.PermissionsValidator;
				}

				attribute.Validate(CurrentContext);
			}
		}
	}

	public abstract class Job<TResult> : JobBase
	{
		public Func<TResult> Validate(ICurrentContext currentContext)
		{
			ValidateInternal(currentContext);

			return () => ExecuteAsync().GetAwaiter().GetResult();
		}

		protected abstract Task<TResult> ExecuteAsync();
	}

	public abstract class Job : JobBase
	{
		public Action Validate(ICurrentContext currentContext)
		{
			ValidateInternal(currentContext);

			return () => ExecuteAsync().GetAwaiter().GetResult();
		}

		protected abstract Task ExecuteAsync();
	}
}
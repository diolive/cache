using System;
using System.Threading.Tasks;

using DioLive.Cache.Common;
using DioLive.Cache.CoreLogic.Exceptions;
using DioLive.Cache.Storage.Contracts;
using DioLive.Cache.Storage.Entities;

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

			ProcessResult(requiredAccess, result);
		}

		protected void AssertUserHasAccessForCategory(int categoryId, ShareAccess requiredAccess)
		{
			IPermissionsValidator permissionsValidator = Settings.PermissionsValidator;
			ResultStatus result = permissionsValidator.CheckUserRightsForCategory(categoryId, CurrentContext.UserId, requiredAccess);

			ProcessResult(requiredAccess, result);
		}

		protected void AssertUserHasAccessForPurchase(Guid purchaseId, ShareAccess requiredAccess)
		{
			IPermissionsValidator permissionsValidator = Settings.PermissionsValidator;
			ResultStatus result = permissionsValidator.CheckUserRightsForPurchase(purchaseId, CurrentContext.UserId, requiredAccess);

			ProcessResult(requiredAccess, result);
		}

		private static void ProcessResult(ShareAccess requiredAccess, ResultStatus result)
		{
			switch (result)
			{
				case ResultStatus.NotFound:
					throw new NotFoundException("Not found");

				case ResultStatus.Success:
					return;

				case ResultStatus.Forbidden:
					throw new ValidationException($"User has no permissions");

				case ResultStatus.Error:
					throw new InvalidOperationException("Unexpected error occured");

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public abstract class Job<TResult> : JobBase
	{
		public Func<TResult> Validate(ICurrentContext currentContext)
		{
			CurrentContext = currentContext;

			Validation();

			return () => ExecuteAsync().GetAwaiter().GetResult();
		}

		protected abstract void Validation();

		protected abstract Task<TResult> ExecuteAsync();
	}

	public abstract class Job : JobBase
	{
		public Action Validate(ICurrentContext currentContext)
		{
			CurrentContext = currentContext;

			Validation();

			return () => ExecuteAsync().GetAwaiter().GetResult();
		}

		protected abstract void Validation();

		protected abstract Task ExecuteAsync();
	}
}
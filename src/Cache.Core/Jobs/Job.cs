using System.Reflection;

using DioRed.Cache.Domain;
using DioRed.Cache.Domain.Entities;
using DioRed.Cache.Core.Attributes;
using DioRed.Cache.Core.Exceptions;

using DioRed.Common;

namespace DioRed.Cache.Core.Jobs;

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
        get => _currentContext
            ?? throw new InvalidOperationException(
                "Current context was not provided by Validate() method"
            );
        set => _currentContext = value;
    }

    protected Guid CurrentBudget => CurrentContext.BudgetId
        ?? throw new InvalidOperationException("Budget was not opened");

    protected void AssertUserIsAuthenticated()
    {
        if (string.IsNullOrEmpty(CurrentContext.GetUserId()))
        {
            throw new ValidationException("User should be authenticated");
        }
    }

    protected void AssertUserHasAccessForBudget(Guid budgetId, ShareAccess requiredAccess)
    {
        Result result = Settings.PermissionsValidator.CheckUserRightsForBudget(
            budgetId,
            CurrentContext.GetUserId(),
            requiredAccess
        );

        ValidationException.RaiseIfNeeded(result);
    }

    protected void AssertCategoryIsInCurrentBudget(int categoryId)
    {
        Category category = Settings.StorageCollection.Categories.GetAsync(categoryId).GetAwaiter().GetResult()
            ?? throw new NotFoundException("Category not found");

        if (category.BudgetId != CurrentBudget)
        {
            ValidationException.RaiseForbidden();
        }
    }

    protected void AssertPurchaseIsInCurrentBudget(Guid purchaseId)
    {
        Purchase purchase = Settings.StorageCollection.Purchases.GetAsync(purchaseId).GetAwaiter().GetResult()
            ?? throw new NotFoundException("Purchase not found");

        if (purchase.BudgetId != CurrentBudget)
        {
            ValidationException.RaiseForbidden();
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
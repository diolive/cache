using DioRed.Cache.Domain;
using DioRed.Cache.Core.Exceptions;

namespace DioRed.Cache.Core.Attributes;

public abstract class ValidationAttribute : Attribute
{
    protected static Guid CurrentBudget(ICurrentContext currentContext)
    {
        return currentContext.BudgetId
            ?? throw new NotFoundException("Budget not found");
    }

    public abstract void Validate(ICurrentContext currentContext);
}
using Microsoft.AspNetCore.Mvc;

namespace ExpensesCalculator.WebAPI.Filters;

public class ValidateResourceAccessAttribute : TypeFilterAttribute
{
    public ValidateResourceAccessAttribute(ResourceType resourceType)
        : base(typeof(ResourceAccessFilter))
    {
        Arguments = new object[] { resourceType };
    }
}

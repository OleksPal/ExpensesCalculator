using ExpensesCalculator.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ExpensesCalculator.WebAPI.Filters;

public class ResourceAccessFilter : IAsyncActionFilter
{
    private readonly IResourceAuthorizationService _authService;
    private readonly ResourceType _resourceType;
    private readonly ILogger<ResourceAccessFilter> _logger;

    public ResourceAccessFilter(
        IResourceAuthorizationService authService,
        ResourceType resourceType,
        ILogger<ResourceAccessFilter> logger)
    {
        _authService = authService;
        _resourceType = resourceType;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        // Get the userName from the HTTP context
        var userName = context.HttpContext.User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(userName))
        {
            _logger.LogWarning("Unauthorized access attempt - no username in claims");
            context.Result = new UnauthorizedResult();
            return; // Short-circuit the pipeline
        }

        // Try to get the resource ID from route/parameters
        // Try multiple parameter name patterns (id, checkId, dayExpensesId)
        Guid resourceId = Guid.Empty;

        var parameterNames = new[] { "id", "checkId", "dayExpensesId" };
        foreach (var paramName in parameterNames)
        {
            if (context.ActionArguments.TryGetValue(paramName, out var idObj) && idObj is Guid paramValue)
            {
                resourceId = paramValue;

                // If we found checkId or dayExpensesId, validate against the appropriate resource
                if (paramName == "checkId" && _resourceType == ResourceType.Check)
                {
                    // For endpoints like GetAllCheckItems that take checkId parameter
                    try
                    {
                        await _authService.ValidateUserAccessToCheck(resourceId, userName);
                        await next();
                        return;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        _logger.LogWarning("User {UserName} doesn't have access to check {CheckId}: {Message}",
                            userName, resourceId, ex.Message);
                        context.Result = new ForbidResult();
                        return;
                    }
                    catch (KeyNotFoundException ex)
                    {
                        _logger.LogWarning("Check {CheckId} not found: {Message}", resourceId, ex.Message);
                        context.Result = new NotFoundObjectResult(new { message = "Resource not found" });
                        return;
                    }
                }
                else if (paramName == "dayExpensesId" && _resourceType == ResourceType.DayExpenses)
                {
                    // For endpoints like GetAllDayExpensesChecks that take dayExpensesId parameter
                    try
                    {
                        await _authService.ValidateUserAccessToDayExpenses(resourceId, userName);
                        await next();
                        return;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        _logger.LogWarning("User {UserName} doesn't have access to day expenses {DayExpensesId}: {Message}",
                            userName, resourceId, ex.Message);
                        context.Result = new ForbidResult();
                        return;
                    }
                    catch (KeyNotFoundException ex)
                    {
                        _logger.LogWarning("Day expenses {DayExpensesId} not found: {Message}", resourceId, ex.Message);
                        context.Result = new NotFoundObjectResult(new { message = "Resource not found" });
                        return;
                    }
                }

                break;
            }
        }

        // For PUT/POST with DTOs, try to get Id from the DTO object
        if (resourceId == Guid.Empty)
        {
            // Look for any parameter that has an "Id" property
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg != null)
                {
                    var idProperty = arg.GetType().GetProperty("Id");
                    if (idProperty != null && idProperty.PropertyType == typeof(Guid))
                    {
                        resourceId = (Guid)idProperty.GetValue(arg);
                        break;
                    }

                    // Also check for CheckId (for Items) or DayExpensesId (for Checks)
                    var checkIdProperty = arg.GetType().GetProperty("CheckId");
                    if (checkIdProperty != null && checkIdProperty.PropertyType == typeof(Guid))
                    {
                        var checkId = (Guid)checkIdProperty.GetValue(arg);
                        // For items being created, we need to validate access to the check
                        if (_resourceType == ResourceType.Item && checkId != Guid.Empty)
                        {
                            try
                            {
                                await _authService.ValidateUserAccessToCheck(checkId, userName);
                                await next(); // Continue to the action
                                return;
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                _logger.LogWarning("User {UserName} doesn't have access to check {CheckId}: {Message}",
                                    userName, checkId, ex.Message);
                                context.Result = new ForbidResult();
                                return;
                            }
                            catch (KeyNotFoundException ex)
                            {
                                _logger.LogWarning("Check {CheckId} not found: {Message}", checkId, ex.Message);
                                context.Result = new NotFoundObjectResult(new { message = "Resource not found" });
                                return;
                            }
                        }
                    }

                    var dayExpensesIdProperty = arg.GetType().GetProperty("DayExpensesId");
                    if (dayExpensesIdProperty != null && dayExpensesIdProperty.PropertyType == typeof(Guid))
                    {
                        var dayExpensesId = (Guid)dayExpensesIdProperty.GetValue(arg);
                        // For checks being created, we need to validate access to the day expenses
                        if (_resourceType == ResourceType.Check && dayExpensesId != Guid.Empty)
                        {
                            try
                            {
                                await _authService.ValidateUserAccessToDayExpenses(dayExpensesId, userName);
                                await next(); // Continue to the action
                                return;
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                _logger.LogWarning("User {UserName} doesn't have access to day expenses {DayExpensesId}: {Message}",
                                    userName, dayExpensesId, ex.Message);
                                context.Result = new ForbidResult();
                                return;
                            }
                            catch (KeyNotFoundException ex)
                            {
                                _logger.LogWarning("Day expenses {DayExpensesId} not found: {Message}", dayExpensesId, ex.Message);
                                context.Result = new NotFoundObjectResult(new { message = "Resource not found" });
                                return;
                            }
                        }
                    }
                }
            }
        }

        if (resourceId == Guid.Empty)
        {
            _logger.LogWarning("Invalid or missing resource ID in request");
            context.Result = new BadRequestObjectResult(new { message = "Invalid or missing resource ID" });
            return;
        }

        try
        {
            // Validate access based on resource type
            switch (_resourceType)
            {
                case ResourceType.Item:
                    await _authService.ValidateUserAccessToItem(resourceId, userName);
                    break;
                case ResourceType.Check:
                    await _authService.ValidateUserAccessToCheck(resourceId, userName);
                    break;
                case ResourceType.DayExpenses:
                    await _authService.ValidateUserAccessToDayExpenses(resourceId, userName);
                    break;
            }

            // Authorization passed - continue to action
            await next();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("User {UserName} doesn't have access to {ResourceType} {ResourceId}: {Message}",
                userName, _resourceType, resourceId, ex.Message);
            context.Result = new ForbidResult();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("{ResourceType} {ResourceId} not found: {Message}",
                _resourceType, resourceId, ex.Message);
            context.Result = new NotFoundObjectResult(new { message = "Resource not found" });
        }
    }
}

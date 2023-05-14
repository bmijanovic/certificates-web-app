using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;

public class AuthorizationMiddleware : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        // If the authorization was forbidden and the resource had a specific requirement,
        // provide a custom 404 response.
        if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure!.FailedRequirements
            .OfType<ClaimsAuthorizationRequirement>().Any(failedClaim => failedClaim.ClaimType == "TwoFactor"))
        {
            // Return a 404 to make it appear as if the resource doesn't exist.
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            return;
        }
        else if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure!.FailedRequirements
            .OfType<ClaimsAuthorizationRequirement>().Any(failedClaim => failedClaim.ClaimType == "PasswordExpired")) {
            context.Response.StatusCode = StatusCodes.Status406NotAcceptable;
            return;
        }

        // Fall back to the default implementation.
        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
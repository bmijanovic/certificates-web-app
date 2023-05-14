using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using System.Text.Json;

public class AuthorizationMiddleware : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {

        if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure!.FailedRequirements
            .OfType<ClaimsAuthorizationRequirement>().Any(failedClaim => failedClaim.ClaimType == "TwoFactor"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            String reason = "TwoFactor";
            await context.Response.WriteAsJsonAsync(new { reason }, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return;
        }
        else if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure!.FailedRequirements
            .OfType<ClaimsAuthorizationRequirement>().Any(failedClaim => failedClaim.ClaimType == "PasswordExpired"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            String reason = "PasswordExpired";
            await context.Response.WriteAsJsonAsync(new { reason }, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return;
        }

        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
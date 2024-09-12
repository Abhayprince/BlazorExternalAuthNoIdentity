using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace BlazorExternalAuthGoogle;

public static class SigninWithGoogleEndpoints
{
    public static IEndpointRouteBuilder MapSigninWithGoogleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/login-google", () =>
        {
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = "/api/after-login-google"
            };
            return TypedResults.Challenge(authProperties, [GoogleDefaults.AuthenticationScheme]);
        });

        app.MapGet("/api/after-login-google", async (HttpContext httpContext) =>
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                // We will register/login this google user into our system
                // In our system there should be some UserId mapped with this googld id/email

                //http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress
                var emailFromGoogleClaims = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (string.IsNullOrWhiteSpace(emailFromGoogleClaims))
                {
                    // Handle this
                    return Results.Unauthorized();
                }
                // Regioster this user with this email into your database if not exists already
                // or login with this email 

                int userId = 123; // Get this from database on the basis of the email id

                // add this user id to existing httpcontext identioty claims

                Claim[] newClaims = [.. httpContext.User.Claims, new Claim(Constants.Claims.UserId, userId.ToString())];

                var newIdentity = new ClaimsIdentity(newClaims, Constants.Auth.AuthScheme);

                var newPrincipal = new ClaimsPrincipal(newIdentity);

                await httpContext.SignInAsync(Constants.Auth.AuthScheme, newPrincipal);

            }
            return Results.Redirect("/");
        });

        return app;
    }
}

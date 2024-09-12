using BlazorExternalAuthGoogle;
using BlazorExternalAuthGoogle.Components;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

builder.Services.AddAuthentication(Constants.Auth.AuthScheme)
    .AddCookie(Constants.Auth.AuthScheme, cookieOptions =>
    {
        cookieOptions.Cookie.Name = ".ap.user";
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;

        googleOptions.SignInScheme = Constants.Auth.AuthScheme;

        googleOptions.AccessDeniedPath = "/external-login-denied";
    });
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication()
    .UseAuthorization();


app.UseAntiforgery();

app.MapRazorComponents<App>();

app.MapSigninWithGoogleEndpoints();

app.Run();

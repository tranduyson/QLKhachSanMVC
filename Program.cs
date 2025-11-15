using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Require authentication for most pages: if AuthUser cookie is missing, redirect to login.
app.Use(async (context, next) =>
{
    var path = context.Request.Path;

    // Allow static assets, Account pages (login), and API endpoints to be accessed without the cookie
    if (path.StartsWithSegments("/css") || path.StartsWithSegments("/js") || path.StartsWithSegments("/lib") ||
        path.StartsWithSegments("/favicon.ico") || path.StartsWithSegments("/_framework") ||
        path.StartsWithSegments("/Account") || path.StartsWithSegments("/api"))
    {
        await next();
        return;
    }

    if (!context.Request.Cookies.ContainsKey("AuthUser"))
    {
        var returnUrl = context.Request.Path + context.Request.QueryString;
        var loginUrl = "/Account/Login" + (string.IsNullOrEmpty(returnUrl) ? "" : "?returnUrl=" + System.Net.WebUtility.UrlEncode(returnUrl));
        context.Response.Redirect(loginUrl);
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


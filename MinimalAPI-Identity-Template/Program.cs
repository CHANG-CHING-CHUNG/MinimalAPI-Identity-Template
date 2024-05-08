using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinimalApiIdentityTemplate;
using MinimalApiIdentityTemplate.Endpoints;
using MinimalApiIdentityTemplate.ExampleModels;
using MinimalApiIdentityTemplate.Extensions;
using MinimalApiIdentityTemplate.Services;

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SampleDbConnection") ??
                       "Data Source=minimalApiExample.db";
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(builder.Configuration["AllowOrigins"] ?? "https://localhost:3000").AllowAnyHeader()
                .AllowAnyMethod().AllowCredentials();
        });
});
builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
        options.SignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddCookie();
builder.Services.AddAuthorization(options => { options.AddPolicy("Admin", policy => policy.RequireRole("Admin")); });
builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "MinimalApiIdentityTemplate";
    options.LoginPath = new PathString("/api/identity/login");
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
ConfigurationHelper.Initialize(builder.Configuration);
var app = builder.Build();

app.UseCors(myAllowSpecificOrigins);
app.UseAuthorization();
app.UseAntiforgery();
// Seeding Roles and a Super User
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "PowerUser", "User" };

    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var user = new AppUser
        { UserName = "super001@test.com", Email = "super001@test.com", NickName = "super001" };
    if (await userManager.FindByEmailAsync(user.Email) == null)
    {
        await userManager.CreateAsync(user, "@0Super001");
        await userManager.AddToRoleAsync(user, "Admin");
    }
}

app.UseContentTypeValidator();
app.MapGet("/antiforgery/token", (IAntiforgery forgeryService, HttpContext context) =>
{
    var tokens = forgeryService.GetAndStoreTokens(context);
    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
        new CookieOptions { HttpOnly = false });

    return Results.Ok();
}).RequireAuthorization();

app.MapGroup("/api/identity")
    .MapUserApi()
    .MapRoleApi()
    .MapLoginApi()
    .MapForgetPasswordApi()
    .MapEmailLoginApi();

app.MapGet("/antiforgery/test", () => Results.Ok(new
{
    Status = "Success",
    Data = new List<object> { new { Message = "CSRF token validated successfully" } }
})).ValidateAntiforgery().RequireAuthorization().WithOpenApi(operation => new OpenApiOperation(operation)
{
    Summary = "Test CSRF token",
    Description = "Check if CSRF token in the header is valid or not"
}).Produces<ResponseErrorWrapper<ErrorExample[]>>(StatusCodes.Status400BadRequest);

app.MapGet("/account/google-login", (SignInManager<AppUser> signInManager, string returnUrl = "") =>
{
    var properties = new AuthenticationProperties
    {
        RedirectUri = $"/account/google-callback?returnUrl={returnUrl}"
    };
    var prop = signInManager.ConfigureExternalAuthenticationProperties("Google", properties.RedirectUri);
    return Results.Challenge(prop, new[] { "Google" });
}).WithTags("Google Login");

// 新增 Google 登入回調端點
app.MapGet("/account/google-callback",
    async (SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, string returnUrl = "") =>
    {
        var info = await signInManager.GetExternalLoginInfoAsync();
        var clientHttpsUrl = ConfigurationHelper.config.GetSection("clientUrls:https").Value;
        if (info == null) return Results.Redirect($"{clientHttpsUrl}/login");

        var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
            false, true);
        if (signInResult.Succeeded)
        {
            Console.WriteLine("ExternalLoginSignInAsync Succeeded");
            return Results.Redirect($"{clientHttpsUrl}{returnUrl}");
        }

        if (signInResult.IsLockedOut) return Results.BadRequest("Google login is locked out");

        var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var user = await userManager.FindByEmailAsync(email);
        if (string.IsNullOrEmpty(email)) return Results.BadRequest("Google login failed");

        if (user != null)
        {
            var result = await userManager.AddLoginAsync(user, info);
            if (!result.Succeeded) return Results.BadRequest("Google login failed");
            await signInManager.SignInAsync(user, false);
            Console.WriteLine("AddLoginAsync + SignInAsync succeeded");
            return Results.Redirect($"{clientHttpsUrl}{returnUrl}");
        }
        else
        {
            user = new AppUser
            {
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                Email = info.Principal.FindFirstValue(ClaimTypes.Email)
            };
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded) return Results.BadRequest("Google login failed");
            await userManager.AddToRoleAsync(user, "User");
            result = await userManager.AddLoginAsync(user, info);
            if (!result.Succeeded) return Results.BadRequest("Google login failed");
            await signInManager.SignInAsync(user, false);
            Console.WriteLine("CreateAsync + AddLoginAsync +  SignInAsync succeeded");
            return Results.Redirect($"{clientHttpsUrl}{returnUrl}");
        }
    }).WithTags("Google Login");
// Google 登出端點
app.MapPost("/account/google-logout", async (HttpContext httpContext, SignInManager<AppUser> signInManager) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);
    await signInManager.SignOutAsync();
    return Results.Ok(new
    {
        Status = "Success",
        Data = new List<object> { new { Message = "Logout succeeded" } }
    });
}).WithTags("Google Login").Produces<ResponseWrapper<MessageExample[]>>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");
app.Run();
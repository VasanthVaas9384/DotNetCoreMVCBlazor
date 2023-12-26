using AspNetCoreMVC.Models.AppDBContext;
using AspNetCoreMVC.Models.AppDBContext.ExtendIdentityUser;
using AspNetCoreMVC.Models.RepositoryPatterns;
using AspNetCoreMVC.Security.CustomAuthorization;
using AspNetCoreMVC.Security.TokenLifespan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add MVC
builder.Services.AddMvc(config =>
{
    //Adding Authorization globally
    var policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});

//Register Google OAuth External Identity Login
builder.Services.AddAuthentication()
        .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = "915613065600-84dm72jfk4h2ur4grnblo6al33ce2q15.apps.googleusercontent.com";
            googleOptions.ClientSecret = "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";
        })
        .AddFacebook(facebookOptions =>
        {
            facebookOptions.AppId = "915613065600-84dm72jfk4h2ur4grnblo6al33ce2q15.apps.googleusercontent.com";
            facebookOptions.AppSecret = "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";
        });



//If any unauthorized resource is tried to access then 
//by default asp .net core redirects to  /Account/AccessDenied
//changing this default path
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
});

//Register Claims Based Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));

    //Using Role as Policy
    //Role as policy becuase in aspnetcore role is a claim of type role
    //options.AddPolicy("SuperAdmin", policy => policy.RequireRole("Super Admin, User"));

    //Register Custom Authorization
    options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
});

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();


//Register DBContext service
builder.Services.AddDbContext<AppDbContext>(x =>
x.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));


//Register Identity Service
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    //Override Password default settings
    options.Password.RequiredLength = 10;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;

    //Block If Email not confirmed
    options.SignIn.RequireConfirmedEmail = true;

    //Provide Custom token name
    options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
}).
    AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");

// Change default lifespan of all generated tokens from 1 day to 5 hours
builder.Services.Configure<DataProtectionTokenProviderOptions>( o =>
 o.TokenLifespan = TimeSpan.FromHours(5)
);

//Change lifespan of particular token(Email Confirmation token) by adding custom token provider & options in security folder
builder.Services.Configure<CustomEmailConfirmationTokenProviderOptions>(o =>
o.TokenLifespan = TimeSpan.FromDays(5));

//Register RepositoryPattern service
builder.Services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();

//Register Custom Authorization
builder.Services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}
app.UseHsts();


//For UnhandledException
app.UseExceptionHandler("/Error");

//For StatusCode
app.UseStatusCodePagesWithRedirects("/Error/{0}");
//Even this gives same result
//app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();


//app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

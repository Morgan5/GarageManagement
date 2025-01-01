using Microsoft.AspNetCore.Authentication.Cookies;
using GarageManagement.FrontOffice.Services;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services nécessaires
builder.Services.AddControllersWithViews();

// Ajouter les services d'authentification
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

// Ajouter les custom services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<VehicleService>();
builder.Services.AddScoped<AppointmentService>();

var app = builder.Build();

// Configurer le pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Utiliser l'authentification et l'autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
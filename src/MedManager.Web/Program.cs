using MedManager.Application.Interfaces;
using MedManager.Application.Services;
using MedManager.Infrastructure;
using MedManager.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Infrastructure services (DbContext, Identity, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application services
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IMedicineManagementService, MedicineManagementService>();
builder.Services.AddScoped<IPatientManagementService, PatientManagementService>();
builder.Services.AddScoped<IAllergyManagementService, AllergyManagementService>();

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
  options.AddPolicy("DoctorOnly", policy => policy.RequireRole("Doctor"));
  options.AddPolicy("PatientOnly", policy => policy.RequireRole("Patient"));
  options.AddPolicy("AdminOrDoctor", policy => policy.RequireRole("Admin", "Doctor"));
});

// Configure cookie settings for better UX
builder.Services.ConfigureApplicationCookie(options =>
{
  options.LoginPath = "/Account/Login";
  options.LogoutPath = "/Account/Logout";
  options.AccessDeniedPath = "/Account/AccessDenied";
  options.ExpireTimeSpan = TimeSpan.FromHours(24);
  options.SlidingExpiration = true;
  options.Cookie.HttpOnly = true;
  options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
  options.Cookie.SameSite = SameSiteMode.Lax;
});

var app = builder.Build();

// Initialize database with seed data
await app.Services.InitializeDatabaseAsync(app.Environment.EnvironmentName);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}
else
{
  app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication & Authorization must be in this order
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Area routes (if needed later)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.Run();

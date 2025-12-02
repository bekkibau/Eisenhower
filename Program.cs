using Microsoft.EntityFrameworkCore;
using Eisenhower.Data;
using Eisenhower.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure SQLite database - use user data directory for packaged app
string dbPath;
if (builder.Environment.IsProduction())
{
    // Store database in user's Application Support folder for macOS app
    var appDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Eisenhower"
    );
    Directory.CreateDirectory(appDataPath);
    dbPath = Path.Combine(appDataPath, "eisenhower.db");
}
else
{
    dbPath = "eisenhower.db";
}

var connectionString = $"Data Source={dbPath}";
builder.Services.AddDbContext<EisenhowerDbContext>(options =>
    options.UseSqlite(connectionString));

// Register services
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IExportImportService, ExportImportService>();

var app = builder.Build();

// Ensure database is created and apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EisenhowerDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Only use HTTPS redirection in development (production app runs on localhost HTTP)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

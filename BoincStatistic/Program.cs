using BoincStatistic.Database;
using BoincStatistic.Database.BoincStats;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var typeOfContent = typeof(Program);
builder.Services.AddDbContext<PostgreSqlContext>(
    opt => opt.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSqlConnection"),
        b => b.MigrationsAssembly(typeOfContent.Assembly.GetName().Name)
    )
);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IBoincStatsRepository, BoincStatsRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
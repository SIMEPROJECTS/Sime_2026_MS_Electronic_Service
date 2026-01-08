using MicroservicesEcosystem.DependyInjection;
using MicroservicesEcosystem.Exceptions;
using MicroservicesEcosystem.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc(options => {
    options.Filters.Add(typeof(ErrorHandlingFilter));
});
builder.Services.AddDbContext<EcosystemBaseDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DataBaseConnection")));
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // Escuchar en el puerto 80 en todas las interfaces
});
Repository.Inject(builder.Services);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

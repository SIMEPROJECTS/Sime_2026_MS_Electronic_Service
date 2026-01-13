using MicroservicesEcosystem.Authentication;
using MicroservicesEcosystem.DependyInjection;
using MicroservicesEcosystem.Exceptions;
using MicroservicesEcosystem.Models;
using MicroservicesEcosystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var key = "yIuZ7Q6Kp4Sv12tT5PwEzrWmL3BhVfGcJnUqWjZdRg1S5cP3yHsGfJjMnUq3tWm";
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc(options => {
    options.Filters.Add(typeof(ErrorHandlingFilter));
});
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false


    };
});
builder.Services.AddSingleton<IJwtAuthenticationManager>(new JwtAuthenticationManager(key)); ;
builder.Services.AddDbContext<EcosystemBaseDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DataBaseConnection")));
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // Escuchar en el puerto 80 en todas las interfaces
});

builder.Services.AddHttpClient<SignService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
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

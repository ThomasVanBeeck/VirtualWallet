using Microsoft.EntityFrameworkCore;
using VirtualWallet.Models;
using VirtualWallet.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Cross-origin resource sharing toestaan voor lokaal Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalAngular",
        policy => policy
            .WithOrigins("http://localhost:4200") // Angular default poort
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddScoped<WalletRepository>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("LocalAngular");
app.MapControllers();
app.Run();
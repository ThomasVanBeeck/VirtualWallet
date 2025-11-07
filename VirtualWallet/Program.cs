using Microsoft.EntityFrameworkCore;
using VirtualWallet.Repositories;
using VirtualWallet.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Cross-origin resource sharing toestaan voor lokaal Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalAngular",
        policy => policy
            .WithOrigins("http://localhost:4200") // Angular default poort
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddControllers();

//builder.Services.AddScoped<WalletRepository>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        DbSeeder.Seed(db);
    }
}

app.UseHttpsRedirection();
app.UseCors("LocalAngular");
app.MapControllers();
app.Run();
using Microsoft.EntityFrameworkCore;
using VirtualWallet.Repositories;
using VirtualWallet.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(Program));

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
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {

        options.Cookie.Name = "VirtualWalletAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.None;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/api/user/login"; // Wordt zelden gebruikt bij API's
        options.AccessDeniedPath = "/api/user/accessdenied";
    });

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
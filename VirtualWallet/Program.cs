using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using VirtualWallet.Interfaces;
using VirtualWallet.Repositories;
using VirtualWallet.Schedulers;
using VirtualWallet.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalAngular",
        policy => policy
            .WithOrigins("http://localhost:4200") // Angular standaard poort
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddScoped<IHoldingRepository, HoldingRepository>();
// holding service is (op dit moment) nog leeg, dus niet nodig
//builder.Services.AddScoped<HoldingService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<StockService>();

builder.Services.AddScoped<ITransferRepository, TransferRepository>();
builder.Services.AddScoped<TransferService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<WalletService>();

builder.Services.AddScoped<IScheduleTimerRepository, ScheduleTimerRepository>();
builder.Services.AddScoped<ISettingsService, ScheduleTimerService>();

builder.Services.AddHostedService<StockUpdateScheduler>();

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {

        options.Cookie.Name = "VirtualWalletAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.None;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/api/user/login";
        options.AccessDeniedPath = "/api/user/accessdenied";
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    using (var scope = app.Services.CreateScope())
    {
        //var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //DbSeeder.Seed(db);
    }
}

app.UseHttpsRedirection();
app.UseCors("LocalAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
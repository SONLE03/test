using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Test;

var builder = WebApplication.CreateBuilder(args);

// Get PORT from environment variable (Railway uses dynamic ports)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8081";
builder.WebHost.UseUrls($"http://*:{port}");

// Use the ConnectionHelper to get the connection string
var connectionString = ConnectionHelper.GetConnectionString(builder.Configuration);
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseNpgsql(connectionString));

// Add Identity services
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders();

builder.Services.AddControllers();  // Configure services for controllers

builder.Services.AddAuthorization();

// Swagger configuration (for API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var redisHost = Environment.GetEnvironmentVariable("REDISHOST"); // Lấy host của Redis
Console.WriteLine(redisHost);
var redisPort = Environment.GetEnvironmentVariable("REDISPORT") ?? "6379";    // Lấy port của Redis (default 6379)
Console.WriteLine(redisPort);
var redisPassword = Environment.GetEnvironmentVariable("REDIS_PASSWORD");
Console.WriteLine(redisPassword);
var redisConnectionString = $"{redisHost}:{redisPort},password={redisPassword}";
Console.WriteLine(redisConnectionString);

var options = ConfigurationOptions.Parse(redisConnectionString);
options.AbortOnConnectFail = false;  // Cho phép retry khi kết nối thất bại
try
{
    // Connect to Redis
    var redisConnection = ConnectionMultiplexer.Connect(options);

    builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);
}
catch (Exception ex)
{
    Console.WriteLine($"Lỗi kết nối Redis: {ex.Message}");
    throw;
}
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRedisCacheService, RedisCacheServiceImp>();


var app = builder.Build();

// Enable Swagger in development
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply migrations and create the database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    await DataHelper.ManageDataAsync(scope.ServiceProvider);
}

// CORS settings (adjust as necessary for your use case)
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

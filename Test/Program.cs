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
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_URL"); // Add abortConnect=false to allow retry
Console.WriteLine($"Đang cố gắng kết nối bằng: {redisConnectionString}");
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

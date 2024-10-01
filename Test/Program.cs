using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Test;

var builder = WebApplication.CreateBuilder(args);

// Lấy PORT từ biến môi trường (Railway sử dụng port động)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8081";
builder.WebHost.UseUrls($"http://*:{port}");

// Lấy DATABASE_URL từ môi trường
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (databaseUrl is not null)
{
    // Chuyển đổi DATABASE_URL từ định dạng URL sang định dạng kết nối của Npgsql
    var connectionString = ConvertPostgresConnectionString(databaseUrl);
    builder.Services.AddDbContext<ApplicationDBContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // Fallback to local config or other default settings if necessary
    builder.Services.AddDbContext<ApplicationDBContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

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
builder.Services.AddControllers();  // Thêm dòng này để cấu hình dịch vụ cho các controller

builder.Services.AddAuthorization();

// Swagger configuration (for API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger in development
if (app.Environment.IsDevelopment())
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

/// <summary>
/// Converts DATABASE_URL format to a PostgreSQL connection string.
/// </summary>
static string ConvertPostgresConnectionString(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    return $"Host={uri.Host};Port={uri.Port};Username={userInfo[0]};Password={userInfo[1]};Database={uri.AbsolutePath.TrimStart('/')};SSL Mode=Require;Trust Server Certificate=true";
}

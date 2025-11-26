using Microsoft.EntityFrameworkCore;
using RentifyApi.Data;
using RentifyApi.Repositories;
using RentifyApi.Services;
using RentifyApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();   // .NET 9 OpenAPI helper

// Configure MySQL
var conn = builder.Configuration.GetConnectionString("DefaultConnection")??throw new Exception("No connection string!");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn))
);

// DI
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IPropertyService, PropertyService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Properties.Any())
    {
        db.Properties.AddRange(new[] {
            new Property { Title="Cozy House", Description="1BR near market", Address="Phnom Penh", PricePerNight=15 },
            new Property { Title="River Apartment", Description="2BR with river view", Address="Siem Reap", PricePerNight=25 }
        });
        db.SaveChanges();
    }
}


app.UseCors("AllowAll");
app.UseStaticFiles();
app.MapControllers();
app.Run();

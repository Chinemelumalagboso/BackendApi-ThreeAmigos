using Microsoft.EntityFrameworkCore;
using ProductApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Retrieve connection string from environment variable or fallback to appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var envConnectionString = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");

if (!string.IsNullOrEmpty(envConnectionString))
{
    connectionString = envConnectionString;
}

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
//lets make some changes
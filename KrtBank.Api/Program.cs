using KrtBank.Application.Interfaces;
using KrtBank.Application.Services;
using KrtBank.Domain.Interfaces;
using KrtBank.Infrastructure.Data;
using KrtBank.Infrastructure.Repositories;
using KrtBank.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "KrtBank API", Version = "v1" });
});

// Database
builder.Services.AddDbContext<KrtBankContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Memory Cache
builder.Services.AddMemoryCache();

// Repositories
builder.Services.AddScoped<IContaRepository, ContaRepository>();

// Services
builder.Services.AddScoped<IContaService, ContaService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<KrtBankContext>();
    context.Database.EnsureCreated();
}

app.Run();

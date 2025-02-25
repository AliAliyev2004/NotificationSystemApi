using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NotificationApplication.Services;
using NotificationInfrastructure.Data;
using NotificationInfrastructure.Messaging;
using NotificationDomain.Interfaces;
using NotificationInfrastructure.Repositories;
using NotificationApplication.Mappings; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(NotificationProfile)); 

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddSingleton<RabbitMQProducer>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run(); 
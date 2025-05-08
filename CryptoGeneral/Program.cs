using CryptoGeneral.Services;
using CryptoGeneral.Services.Implementations;
using CryptoGeneral.Services.Interfaces;
using UserService.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Middleware для исключений
builder.Services.AddTransient<ExceptionsMiddleware>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Сервисы
builder.Services.AddScoped<IDesService, DesService>();
builder.Services.AddScoped<IRsaService, RsaService>();
builder.Services.AddScoped<IMd5Service, Md5Service>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionsMiddleware>();

app.MapControllers();

app.Run();
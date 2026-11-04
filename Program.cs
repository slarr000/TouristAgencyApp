using Serilog;
using TouristAgencyApp.Interfaces;
using TouristAgencyApp.Repositories;
using TouristAgencyApp.Middlewares; // Пространство имен для глобального обработчика ошибок

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка Serilog (Логирование в консоль и в файл по дням)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// 2. Добавление контроллеров
builder.Services.AddControllers();

// 3. Настройка Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Настройка CORS (Разрешаем фронтенду общаться с API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()  // В будущем тут можно указать конкретный адрес React/Vue/Angular
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 5. Регистрация репозиториев (Внедрение зависимостей)
builder.Services.AddScoped<IAgencyRepository, AgencyRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IExcursionBookingRepository, ExcursionBookingRepository>();
builder.Services.AddScoped<IExcursionRepository, ExcursionRepository>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IPassportRepository, PassportRepository>();
builder.Services.AddScoped<ITouristCargoRepository, TouristCargoRepository>();
builder.Services.AddScoped<ITouristChildRepository, TouristChildRepository>();
builder.Services.AddScoped<ITouristRepository, TouristRepository>();

var app = builder.Build();

// --- НАСТРОЙКА КОНВЕЙЕРА ЗАПРОСОВ (Порядок строго обязателен!) ---

// Подключаем наш глобальный перехватчик ошибок
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Включаем CORS перед авторизацией и контроллерами
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
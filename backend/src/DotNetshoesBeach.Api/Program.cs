using DotNetshoesBeach.Application;
using DotNetshoesBeach.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Injeção de Dependências por camadas
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Controllers e Endpoints
builder.Services.AddControllers();

// 3. Configuração de CORS para permitir requisições do front-end Vue.js
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
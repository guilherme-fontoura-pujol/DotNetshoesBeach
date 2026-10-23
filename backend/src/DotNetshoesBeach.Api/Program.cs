using System.Text;
using System.Text.Json.Serialization;
using DotNetshoesBeach.Api.Support;
using DotNetshoesBeach.Application;
using DotNetshoesBeach.Infrastructure;
using DotNetshoesBeach.Infrastructure.Options;
using DotNetshoesBeach.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Injeção de Dependências por camadas
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Autenticação JWT
var jwt = builder.Configuration.GetSection(JwtOptions.Secao).Get<JwtOptions>() ?? new JwtOptions();
if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
{
    throw new InvalidOperationException(
        "Configure 'Jwt:Key' com pelo menos 32 caracteres (user-secrets, variável de ambiente Jwt__Key " +
        "ou appsettings.Development.json). Veja docs/database/README.md.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Mantém os nomes curtos dos claims ("sub", "role") exatamente como foram emitidos.
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = "sub",
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();

// 3. Controllers (enums trafegam como texto: "Admin", "EmManutencao", "Confirmada"...)
builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 4. CORS para o front-end Vue.js
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

// 5. Banco: migrations + constraint anti-sobreposição + admin inicial
if (app.Configuration.GetValue("Database:InicializarAoIniciar", true))
{
    await DbInitializer.InicializarAsync(app.Services);
}

app.UseExceptionHandler();

// Em desenvolvimento o front chama http://localhost:5237 direto; o redirect para HTTPS quebraria o CORS.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

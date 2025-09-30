using Boamesa.Infrastructure;              // BoamesaContext
using Boamesa.Application.Services;        // << adicione: seus services de negócio
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1) EF Core + SQL Server
builder.Services.AddDbContext<BoamesaContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// 2) Controllers (API convencional)
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // Enums como string (opcional, mas ajuda no Swagger/front)
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // Datas já funcionam bem no .NET 8 (DateOnly/TimeOnly suportados)
    });

// 3) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4) CORS (libera o front React local - ajuste a origem conforme sua porta/host)
builder.Services.AddCors(options =>
{
    options.AddPolicy("react",
        policy => policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()); // deixe true só se for usar cookies; para JWT pode tirar
});

// 5) DI dos Services de negócio (IMPORTANTÍSSIMO)
builder.Services.AddScoped<SugestaoDoChefeService>();
builder.Services.AddScoped<ReservaService>();
builder.Services.AddScoped<PedidoService>();

var app = builder.Build();

// 6) Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 7) Middleware simples p/ padronizar erros de regra de negócio (422)
app.Use(async (ctx, next) =>
{
    try
    {
        await next();
    }
    catch (BusinessRuleException bre)
    {
        ctx.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        ctx.Response.ContentType = "application/json; charset=utf-8";
        var body = JsonSerializer.Serialize(new { error = bre.Message });
        await ctx.Response.WriteAsync(body);
    }
});

// CORS antes dos controllers
app.UseCors("react");

// Mapear controllers
app.MapControllers();

app.Run();
// Program.cs (Boamesa.Api)

using Boamesa.Infrastructure;              // BoamesaContext + BoamesaContextSeed
using Boamesa.Application.Services;        // BusinessRuleException + Services
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1) EF Core + SQL Server
builder.Services.AddDbContext<BoamesaContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// 2) Controllers (API convencional)
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // Enums como string (útil p/ Swagger e front)
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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
            .AllowCredentials()); // deixe true só se usar cookies; para JWT pode remover
});

// 5) DI dos Services de negócio
builder.Services.AddScoped<SugestaoDoChefeService>();
builder.Services.AddScoped<ReservaService>();
builder.Services.AddScoped<PedidoService>();

var app = builder.Build();

// 6) Swagger + HTTPS
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// 7) Middleware p/ padronizar BusinessRuleException -> 422
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

// 8) Seed inicial de dados (itens, mesas, usuário, etc.)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BoamesaContext>();
    await BoamesaContextSeed.EnsureSeededAsync(db);
}

// 9) CORS antes dos controllers
app.UseCors("react");

// 10) Mapear controllers
app.MapControllers();

app.Run();
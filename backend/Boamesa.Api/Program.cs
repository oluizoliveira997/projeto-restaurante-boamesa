using Boamesa.Infrastructure;              // BoamesaContext + BoamesaContextSeed
using Boamesa.Application.Services;        // BusinessRuleException + Services + PasswordHasher
using Boamesa.Domain.Entities;             // Usuario
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

// (opcional) Informe a porta HTTPS para evitar warnings de redirecionamento
builder.Services.AddHttpsRedirection(o => o.HttpsPort = 7270);

// 5) DI dos Services de negócio
builder.Services.AddScoped<SugestaoDoChefeService>();
builder.Services.AddScoped<ReservaService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<PaymentService>();

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

// 8) Seed inicial de dados (itens, mesas, e garante 1 usuário demo)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BoamesaContext>();

    // garante que o banco existe
    await db.Database.EnsureCreatedAsync();

    // seu seed existente (mesas, itens, etc.)
    await BoamesaContextSeed.EnsureSeededAsync(db);

    // garante um usuário demo para login fake (email: demo@demo.com / senha: 123456)
    if (!await db.Usuarios.AnyAsync())
    {
        db.Usuarios.Add(new Usuario
        {
            Email = "demo@demo.com",
            SenhaHash = PasswordHasher.Sha256("123456"), // se quiser texto puro, use "123456"
            CriadoEm = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }
}

// 9) CORS antes dos controllers
app.UseCors("react");

// 10) Mapear controllers
app.MapControllers();

app.Run();
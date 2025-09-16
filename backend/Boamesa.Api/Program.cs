using Boamesa.Infrastructure;              // seu DbContext fica nesse projeto/namespace
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1) EF Core + SQL Server
builder.Services.AddDbContext<BoamesaContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// 2) Controllers (API convencional)
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // opcional: enums como string, datas ISO, etc.
        // o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 3) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4) CORS (libera o front React local - ajuste a origem conforme sua porta/host)
builder.Services.AddCors(options =>
{
    options.AddPolicy("react",
        policy => policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173") // Vite default
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS antes dos controllers
app.UseCors("react");

app.MapControllers();   // expõe os controllers (ex.: /api/itens)

app.Run();

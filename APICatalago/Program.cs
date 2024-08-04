using APICatalago.Data;
using APICatalago.DTOS.Mappings;
using APICatalago.Extensions;
using APICatalago.Filters;
using APICatalago.Logging;
using APICatalago.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    }).AddNewtonsoftJson();

// Configuração de CORS para permitir todas as origens, métodos e cabeçalhos
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configuração de serviços adicionais
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();
builder.Services.AddSqlServer<AppDbContext>(builder.Configuration["ConnectionStrings:DefaultConnection"]);
builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(ProductDTOMappingProfile));
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information,
}));

var app = builder.Build();

// Pipeline de configuração HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler(); // Assumindo que este método configura o manuseio de exceções customizado
}

app.UseHttpsRedirection();

// Aplicar o middleware de CORS
app.UseCors("AllowAll");

// Aplicar middleware de autorização
app.UseAuthorization();

// Mapeamento de controladores
app.MapControllers();

app.Run();

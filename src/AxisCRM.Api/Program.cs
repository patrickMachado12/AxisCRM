using System.Security.Claims;
using System.Text;
using AutoMapper;
using FluentValidation;
using AxisCRM.Api.AutoMapper;
using AxisCRM.Api.Data;
using AxisCRM.Api.Domain.Repository.Classes;
using AxisCRM.Api.Domain.Repository.Interfaces;
using AxisCRM.Api.Domain.Services.Classes;
using AxisCRM.Api.Domain.Services.Interfaces;
using AxisCRM.Api.Domain.Validator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AxisCRM.Api.Domain.Validator.ClienteValidator;

var builder = WebApplication.CreateBuilder(args);

ConfigurarServices(builder);

ConfigurarInjecaoDeDependencia(builder);

var app = builder.Build();

ConfigurarAplicacao(app);

app.Run();

static void ConfigurarInjecaoDeDependencia(WebApplicationBuilder builder)
{
    string? connectionString = builder.Configuration.GetConnectionString("DEVELOP");
    
    builder.Services.AddDbContext<ApplicationContext>(options => 
        options.UseNpgsql(connectionString), ServiceLifetime.Transient, ServiceLifetime.Transient);
    var config = new MapperConfiguration(cfg => {
        
        cfg.AddProfile<UsuarioProfile>();
        cfg.AddProfile<ClienteProfile>();
    });

    IMapper mapper = config.CreateMapper();

    builder.Services
    .AddHttpContextAccessor()
    .AddSingleton(builder.Configuration)
    .AddSingleton(builder.Environment)
    .AddSingleton(mapper)
    .AddScoped<TokenService>()
    .AddScoped<IUsuarioRepository, UsuarioRepository>()
    .AddScoped<IUsuarioService, UsuarioService>()
    .AddTransient<UsuarioCadastroValidador>()
    .AddTransient<UsuarioEdicaoValidador>()
    .AddValidatorsFromAssemblyContaining<UsuarioLoginValidador>()
    .AddTransient<IClienteRepository, ClienteRepository>()
    .AddTransient<IClienteService, ClienteService>()
    .AddTransient<ClienteValidador>();
}

static void ConfigurarServices(WebApplicationBuilder builder)
{
    builder.Services
    .AddCors()
    .AddControllers().ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    }).AddNewtonsoftJson();

    builder.Services.AddSwaggerGen(c =>
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JTW Authorization header using the Beaerer scheme (Example: 'Bearer 12345abcdef')",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        c.SwaggerDoc("v1", new OpenApiInfo { Title = "AxisCRM.Api", Version = "v1" });
        c.CustomSchemaIds(type => type.FullName);
        
        c.EnableAnnotations();
    });

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["KeySecret"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            RoleClaimType = ClaimTypes.Role
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Admin", policy =>
            policy.RequireRole("Admin"));
    });
}

static void ConfigurarAplicacao(WebApplication app)
{
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    app.UseDeveloperExceptionPage()
        .UseRouting();

    app.UseSwagger()
        .UseSwaggerUI(c =>
        {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinTech.Api v1");
                c.RoutePrefix = string.Empty;
        });

    app.UseCors(x => x
        .AllowAnyOrigin() 
        .AllowAnyMethod()
        .AllowAnyHeader())
        .UseAuthentication();

    app.UseAuthorization();

    app.UseEndpoints(endpoints => endpoints.MapControllers());

    app.MapControllers();
}

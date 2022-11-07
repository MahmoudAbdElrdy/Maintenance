using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using static System.Collections.Specialized.BitVector32;
using System.Text;
using Common.Options;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Web.Globals;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthDomain.Entities.Auth;
using System.Reflection;
using Web.Profiler;
using Boxed.AspNetCore;
using Maintenance.Infrastructure.Persistence;
using Maintenance.Infrastructure.Persistence.MSSQL;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Maintenance.Domain.Persistence;
using Microsoft.AspNetCore.Mvc;
using Maintenance.Application;
using Maintenance.API.Helper;
using Maintenance.Application.GenericRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using Maintenance.Application.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
AppSettings appSettings = new AppSettings();
builder.Configuration.Bind("SystemSetting", appSettings);
builder.Services.AddSingleton(appSettings);
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

var passwordOption =builder.Configuration?.GetSection(Sections.Password)?.Get<PasswordOption>();
var identityLockoutOption =builder.Configuration?.GetSection(Sections.IdentityLockout)?.Get<IdentityLockoutOption>();

builder.Services.Configure<IdentityOptions>(options => {
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(identityLockoutOption?.DefaultLockoutTimeSpan ?? 5);
    options.Lockout.MaxFailedAccessAttempts = identityLockoutOption?.MaxFailedAccessAttempts ?? 5;
    options.Lockout.AllowedForNewUsers = identityLockoutOption?.AllowedForNewUsers ?? true;

    options.Password.RequiredLength = passwordOption?.RequiredLength ?? 6;
    options.Password.RequireNonAlphanumeric = passwordOption?.RequireNonAlphanumeric ?? false;
    options.Password.RequireLowercase = passwordOption?.RequireLowercase ?? false;
    options.Password.RequireUppercase = passwordOption?.RequireUppercase ?? false;
    options.Password.RequireDigit = passwordOption?.RequireDigit ?? false;
});
//  installers.ForEach(i => i.InstallService(services, Configuration));
//----------------------------jwtSettings-------------------------------------
var jwtSettings = new JwtOption();
builder.Configuration.Bind(nameof(jwtSettings), jwtSettings);
builder.Services.AddSingleton(jwtSettings);
//----------------------------end jwtSettings-------------------------------------
//----------------------------jwtSettings-------------------------------------
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
   // IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
    ValidateIssuer = false,
    ValidateAudience = false,
    RequireExpirationTime = false,
    ValidateLifetime = false
};
builder.Services.AddSingleton(tokenValidationParameters);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.SaveToken = true;
    x.TokenValidationParameters = tokenValidationParameters;
});

var persistenceConfig =builder.Configuration?.GetSection(nameof(Sections.Persistence))?.Get<PersistenceConfiguration>();

if (persistenceConfig?.Provider == "MSSQL")
{
    MaintenanceSqlServiceCollectionExtensions.AddMssqlDbContext(builder.Services, builder.Configuration);
  //  builder.Services.MaintenanceSqlServiceCollectionExtensions(builder.Configuration);
    //services.AddDbContext<AppDbContext, MsSqlAppDbContext>();
}
builder. Services
         .AddIdentity<User, Role>()
         .AddEntityFrameworkStores<AppDbContext>()
         .AddDefaultTokenProviders();
builder.Services
.ConfigureAndValidateSingleton<JwtOption>(builder.Configuration.GetSection(nameof(Sections.JwtOption)))
.ConfigureAndValidateSingleton<AppInfoOption>(builder.Configuration.GetSection(nameof(Sections.AppInfoOption)))
.ConfigureAndValidateSingleton<ImageOption>(builder.Configuration.GetSection(nameof(Sections.ImageOption)))
.ConfigureAndValidateSingleton<AppSettings>(builder.Configuration.GetSection(nameof(Sections.AppSettings)))
;
builder.Services
 .AddAutoMapper(new Assembly[] {
          typeof(AutoMapperProfile).GetTypeInfo().Assembly,
 })
 .AddHttpContextAccessor()
 .AddHttpClient();
ApplicationServiceRegistration.AddApplicationServices(builder.Services);
#region Swagger
builder.Services.AddApiVersioning(config =>
{
    // Specify the default API Version as 1.0
    config.DefaultApiVersion = new ApiVersion(1, 0);
    // If the client hasn't specified the API version in the request, use the default API version number 
    config.AssumeDefaultVersionWhenUnspecified = true;
    // Advertise the API versions supported for the particular endpoint
    config.ReportApiVersions = true;
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Maintenance APIs Reference",
    });

    c.AddSecurityDefinition(
    "token",
    new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Name = HeaderNames.Authorization
    }
        );
    c.AddSecurityRequirement(
    new OpenApiSecurityRequirement
    {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "token"
                            },
                        },
                        Array.Empty<string>()
                    }
    }
        );
});
#endregion Swagger
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));


builder.Services.AddScoped(typeof(IGRepository<>), typeof(GRepository<>));



builder.Services.AddOptions();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, CustomRequireUserClaim>();
builder.Services.AddSignalR();
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

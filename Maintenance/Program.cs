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
using Maintenance.Helpers;
using Maintenance.API.Helpers;
using Maintenance.API.Middleware;
using Refit;
using Maintenance.Domain.Interfaces;
using Maintenance.Application.Interfaces;
using dotnet_6_json_localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCors",
        builder =>
        {
            builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://broadcastmp-001-site3.itempurl.com/")
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials();
        });
});

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

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    //options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(10);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
});
builder.Services.AddAuthorization(options =>
{
});

var persistenceConfig =builder.Configuration?.GetSection(nameof(Sections.Persistence))?.Get<PersistenceConfiguration>();

if (persistenceConfig?.Provider == "MSSQL")
{
    MaintenanceSqlServiceCollectionExtensions.AddMssqlDbContext(builder.Services, builder.Configuration);
  //  builder.Services.MaintenanceSqlServiceCollectionExtensions(builder.Configuration);
    //services.AddDbContext<AppDbContext, MsSqlAppDbContext>();
}
builder. Services
         .AddIdentity<User,Role>()
         .AddEntityFrameworkStores<AppDbContext>()
         .AddDefaultTokenProviders();
builder.Services
.ConfigureAndValidateSingleton<JwtOption>(builder.Configuration.GetSection(nameof(Sections.JwtOption)))
.ConfigureAndValidateSingleton<AppInfoOption>(builder.Configuration.GetSection(nameof(Sections.AppInfoOption)))
.ConfigureAndValidateSingleton<ImageOption>(builder.Configuration.GetSection(nameof(Sections.ImageOption)))
.ConfigureAndValidateSingleton<AppSettings>(builder.Configuration.GetSection(nameof(Sections.AppSettings)));

builder.Services.AddRefitClient<IRoom>()
   .ConfigureHttpClient(c =>
   {

   c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ExternalServices:RoomApi"));
   c.Timeout = TimeSpan.FromMinutes(3);
  
  });

ApplicationServiceRegistration.AddApplicationServices(builder.Services);
builder.Services
 .AddAutoMapper(new Assembly[] {
          typeof(AutoMapperProfile).GetTypeInfo().Assembly,
 })
 .AddHttpContextAccessor()
 .AddHttpClient();
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
    c.OperationFilter<AddRequiredHeaderParameter>();
    c.AddSecurityDefinition(
    "token",
    new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Name = HeaderNames.Authorization,
       
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
var jwtOption = builder.Configuration?.GetSection(nameof(Sections.JwtOption))?.Get<JwtOption>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


}).AddJwtBearer(cfg => {
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwtOption.Issuer,
        ValidAudience = jwtOption.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.Key)),
        ClockSkew = TimeSpan.Zero // remove delay of token when expire
    };



});
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

builder.Services.AddLocalization();
builder.Services.AddSingleton<LocalizationMiddleware>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();


var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{

    serviceScope.ServiceProvider.GetService<AuthorizeByPermissionsAttribute>();
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<MaintenanceSqlContext>();
    var serviceProvider = serviceScope.ServiceProvider;
    if (!serviceScope.ServiceProvider.GetService<MaintenanceSqlContext>().AllMigrationsApplied())
    {
        serviceScope.ServiceProvider.GetService<MaintenanceSqlContext>().Migrate();
    }
   //ma IntegratedRecruitmentContextSeed.Seed(dbContext, serviceProvider);
}

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("./v1/swagger.json", " Camel Club"));
}

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
}
app.UseApiVersioning();
app.UseDeveloperExceptionPage();
app.UseMiddleware<ExceptionMiddleware>();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads")),
    RequestPath = "/wwwroot/Uploads"
});


app.UseCors("AllowCors");
var options = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(new CultureInfo("ar"))
};
app.UseRequestLocalization(options);
app.UseMiddleware<LocalizationMiddleware>();
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

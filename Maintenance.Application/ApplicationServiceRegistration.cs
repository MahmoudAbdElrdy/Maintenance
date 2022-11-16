using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Infrastructure;
using Maintenance.Application.Behaviours;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers;
using Maintenance.Application.Interfaces;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Maintenance.Application
{
    public static class ApplicationServiceRegistration
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
           

            // services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

      services.AddMediatR(Assembly.GetExecutingAssembly());
            // services.AddMediatR(typeof(GetAllSkeleton).GetTypeInfo().Assembly);
      services.AddScoped<IAuditService, AuditService>();
      services.AddScoped<ResponseDTO>();
      
      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(
          Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

     
           return services;
    }
  }
}

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
namespace Maintenance.Helpers
{
    public class AddRequiredHeaderParameter : IOperationFilter
    {
      
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;

            if (descriptor != null && descriptor.ControllerName != null)
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "Accept-Language",
                    In = ParameterLocation.Header,

                    Schema = new OpenApiSchema
                    {
                        Type = "String",
                   //     Default = new OpenApiString("en-US")
                        Default = new OpenApiString("ar-SA")
                    },
                    Required = false
                });
              
            }
        }
    }
}

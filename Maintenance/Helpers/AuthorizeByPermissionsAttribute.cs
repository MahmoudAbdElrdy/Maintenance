using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Maintenance.API.Helpers
{
	public class AuthorizeByPermissionsAttribute: AuthorizeAttribute, IAuthorizationFilter
	{
		public string? _permission { get; }
		public AuthorizeByPermissionsAttribute(string? permission)
		{
			_permission = permission;
		}
		public void OnAuthorization(AuthorizationFilterContext context)
		{

			string permissions = context.HttpContext.User.Claims.Where(c => c.Type == "Permissions").FirstOrDefault().Value;
			if(string.IsNullOrEmpty(permissions) || permissions == "[]")
			{

				context.HttpContext.Response.StatusCode = 401; //Unauthorizedr


				context.Result = new JsonResult("Permission denined!");

			}
			else
			{
				var permissionsList = JsonConvert.DeserializeObject<List<string>>(permissions);
				if(permissionsList.Contains(_permission))
				{
					return;
				}
				else
				{
					context.HttpContext.Response.StatusCode = 401; //Unauthorizedr
					context.Result = new JsonResult("Permission denined!");

				}

			}
		}
	}
}

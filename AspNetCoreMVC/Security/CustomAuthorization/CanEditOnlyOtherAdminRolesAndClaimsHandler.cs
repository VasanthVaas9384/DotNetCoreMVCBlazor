using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AspNetCoreMVC.Security.CustomAuthorization
{
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler:
        AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {

        private readonly IHttpContextAccessor httpContextAccessor;
        public CanEditOnlyOtherAdminRolesAndClaimsHandler(
                IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                ManageAdminRolesAndClaimsRequirement requirement)
        {

            var loggedInAdminId = context.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value.ToString();

            var adminIdBeingEdited = httpContextAccessor.HttpContext
                .Request.Query["userId"].ToString();

            if (context.User.IsInRole("Admin")
                 && context.User.HasClaim(c => c.Type == "Edit Role" && c.Value == "true")
                 && adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

        }
        //protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        //{
        //    //Resource property of AuthorizationHandlerContext returns the resource that we are protecting.
        //    //    In our case, we are using this custom requirement to protect a controller action method. 
        //    //    So the following line returns the controller action being protected as the AuthorizationFilterContext and 
        //    //provides access to HttpContext, RouteData, and everything else provided by MVC and Razor Pages.
           
        //    var authFilterContext = context.Resource as AuthorizationFilterContext;
        //    if (authFilterContext == null)
        //    {
        //        return Task.CompletedTask;
        //    }
        //    string loggedInAdminId =
        //    context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

        //    string adminIdBeingEdited = authFilterContext.HttpContext.Request.Query["UserId"];

        //    if (context.User.IsInRole("Admin") &&
        //   context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
        //   adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
        //    {
        //        context.Succeed(requirement);
        //    }

        //    return Task.CompletedTask;
        //}
    }
}

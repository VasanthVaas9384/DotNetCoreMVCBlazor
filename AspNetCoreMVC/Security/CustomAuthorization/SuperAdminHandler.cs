using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreMVC.Security.CustomAuthorization
{
    public class SuperAdminHandler: AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        {
            if(context.User.IsInRole("Super Admin"))
            {
                context.Succeed(requirement);
            }
           return  Task.CompletedTask;
        }
    }
}

using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace QingRain.IdentityServer.ResourceOwnerPassword
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = Config.Users.Find(u => u.Username == context.UserName && u.Password == context.Password);

            if (user is not null)
            {
                context.Result = new GrantValidationResult(user.SubjectId, "pwd", user.Claims);
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid username or password");
            }

            return Task.CompletedTask;
        }
    }

}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PureCode.Core.Entities;
using PureCode.Core.Managers;
using System.Threading.Tasks;

namespace PureCode.Core.Managers
{
  public class SignInManager : SignInManager<UserEntity>
  {
    public SignInManager(
        UserManager userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<UserEntity> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<UserEntity>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<UserEntity> confirmation
    ) : base(
        userManager,
        contextAccessor,
        claimsFactory,
        optionsAccessor,
        logger,
        schemes,
        confirmation
    )
    { }

    protected override async Task<SignInResult?> PreSignInCheck(UserEntity user)
    {
      if (user.IsDeleted)
      {
        return SignInResult.NotAllowed;
      }

      if (!await CanSignInAsync(user))
      {
        return SignInResult.NotAllowed;
      }
      if (await IsLockedOut(user))
      {
        return await LockedOut(user);
      }
      return null;
    }
  }
}
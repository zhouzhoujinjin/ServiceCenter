using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PureCode.Core.Entities;
using PureCode.Core.Managers;
using PureCode.Core.Models;
using PureCode.Core.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PureCode.Core.Managers
{
  public class TokenManager(UserManager manager, IOptions<JwtIssuerOptions> jwtIssuerOptionsAccessor)
  {
    private readonly JwtIssuerOptions _jwtIssuerOptions = jwtIssuerOptionsAccessor.Value;

    /// <summary>
    /// 只需要 AccessToken，无需 RefreshToken
    /// </summary>
    /// <param name="user"></param>
    /// <param name="rememberMe"></param>
    /// <param name="otherClaimsAction"></param>
    /// <returns></returns>
    public AuthenticationTokens GenerateTokens(UserEntity user, bool rememberMe, Action<UserEntity, List<Claim>>? otherClaimsAction = null)
    {
      return new AuthenticationTokens
      {
        AccessToken = GenerateAccessToken(user, rememberMe, otherClaimsAction)
      };
    }

    /// <summary>
    /// 生成 AccessToken 及 RefreshToken
    /// AccessToken 中会包含用户名（nameId）和用户Id(id)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="rememberMe"></param>
    /// <param name="otherClaimsAction"></param>
    /// <returns></returns>
    public async Task<AuthenticationTokens> GenerateTokensAsync(UserEntity user, bool rememberMe, Action<UserEntity, List<Claim>>? otherClaimsAction = null)
    {
      return new AuthenticationTokens
      {
        AccessToken = GenerateAccessToken(user, rememberMe, otherClaimsAction),
        RefreshToken = await GenerateRefreshTokenAsync(user)
      };
    }

    public string GenerateAccessToken(UserEntity user, bool rememberMe, Action<UserEntity, List<Claim>>? otherClaimsAction = null)
    {
      var claims = new List<Claim> {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!)
      };

      otherClaimsAction?.Invoke(user, claims);

      var userClaimsIdentity = new ClaimsIdentity(claims);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Issuer = _jwtIssuerOptions.Issuer,
        Subject = userClaimsIdentity,
        Audience = _jwtIssuerOptions.Audience,
        Expires = DateTime.Now.Add(rememberMe ? _jwtIssuerOptions.ValidFor * 360 : _jwtIssuerOptions.ValidFor),
        SigningCredentials = _jwtIssuerOptions.SigningCredentials
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var securityToken = tokenHandler.CreateToken(tokenDescriptor);
      var serializeToken = tokenHandler.WriteToken(securityToken);

      return serializeToken;
    }

    public async Task<string> GenerateRefreshTokenAsync(UserEntity user)
    {
      await manager.RemoveAuthenticationTokenAsync(user, "Default", "RefreshToken");
      var newRefreshToken = await manager.GenerateUserTokenAsync(user, "Default", "RefreshToken");
      await manager.SetAuthenticationTokenAsync(user, "Default", "RefreshToken", newRefreshToken);
      return newRefreshToken;
    }

    public async Task<bool> VerifyRefreshTokenAsync(UserEntity user, string refreshToken)
    {
      return await manager.VerifyUserTokenAsync(user, "Default", "RefreshToken", refreshToken);
    }

    public async Task<string?> RefreshAccessToken(UserEntity user, string refreshToken)
    {
      var isValid = await VerifyRefreshTokenAsync(user, refreshToken);
      return isValid ? GenerateAccessToken(user, true) : null;
    }
  }
}
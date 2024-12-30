using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PureCode.Authorizations;
using PureCode.Core.Converters;
using PureCode.Core.Entities;
using PureCode.Core.Kernel;
using PureCode.Core.Managers;
using PureCode.Core.Options;
using PureCode.Core.UserFeature.Managers;
using PureCode.Utils.Captcha;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using PureCode.Core.ProfileFeature.UserQueryFilters;
using PureCode.Core.UserFeature.UserQueryFilters;
using PureCode.Core.UserQueryFilters;
using PureCode.Core.UserFeature.UserQueryFilters.Options;

namespace PureCode.Core.UserFeature;

public static class PureCodeBuilderExtensions
{
  public static PureCodeServiceCollectionBuilder WithUser(this PureCodeServiceCollectionBuilder builder)
  {
    var identityBuilder = builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
      {
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequiredLength = 0;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = false;
        options.ClaimsIdentity.UserNameClaimType = "nameid";
      }).AddRoles<RoleEntity>()
      .AddUserManager<UserManager>()
      .AddSignInManager<SignInManager>()
      .AddRoleManager<RoleManager>()
      .AddDefaultTokenProviders()
      .AddEntityFrameworkStores<PureCodeDbContext>();

    builder.Services.AddSingleton(sp => ProfileManager.ProfileKeyMap);
    builder.Services.AddScoped<ProfileManager>();
    builder.Services.AddScoped<TokenManager>();
    builder.Services.AddScoped<CaptchaManager>();

    builder.Services.AddScoped<IUserQueryFilter, NameFilter>();
    builder.Services.AddScoped<IUserQueryFilter, DeletedUserFilter>();
    builder.Services.AddScoped<IUserQueryFilter, RoleNameFilter>();
    builder.Services.AddScoped<IUserQueryFilter, UserIdsFilter>();
    builder.Services.AddScoped<IUserQueryFilter, UserNameFilter>();
    builder.Services.AddScoped<IUserQueryFilter, VisibleUserFilter>();
    builder.Services.AddScoped<IUserQueryFilter, SearchableProfileFilter>();

    #region Jwt 设置

    var jwtAppSettingOptions = builder.Configuration.GetSection(nameof(JwtIssuerOptions));
    var signingKey =
      new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingOptions[nameof(JwtIssuerOptions.SecretKey)]!));
    builder.Services.Configure<JwtIssuerOptions>(options =>
    {
      options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)]!;
      options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)]!;
      var validFor = jwtAppSettingOptions["AccessTokenExpiredMinutes"];
      if (!string.IsNullOrEmpty(validFor) && int.TryParse(validFor, out var t))
        options.ValidFor = TimeSpan.FromMinutes(t);
      options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
    });

    var tokenValidationParameters = new TokenValidationParameters
    {
      NameClaimType = JwtRegisteredClaimNames.NameId,
      ValidateIssuer = true,
      ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

      ValidateAudience = true,
      ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

      ValidateIssuerSigningKey = true,
      IssuerSigningKey = signingKey,

      RequireExpirationTime = false,
      ValidateLifetime = false,
      ClockSkew = TimeSpan.Zero
    };
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

    builder.Services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
      options.RequireHttpsMetadata = false;
      options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
      options.SaveToken = false;
      options.TokenValidationParameters = tokenValidationParameters;
      options.MapInboundClaims = false;
      options.RequireHttpsMetadata = false;
    });

    #endregion Jwt 设置

    #region PermissionAuthorization 设置

    builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
    builder.Services.AddAuthorization(options =>
    {
      options.AddPolicy(
        PermissionClaimNames.ApiPermission,
        b => b.AddRequirements(new PermissionRequirement())
      );
    });

    #endregion PermissionAuthorization 设置

    #region 验证码设置

    var captchaOptions = builder.Configuration.GetSection(nameof(CaptchaOptions));
    builder.Services.AddHeiCaptcha();
    builder.Services.Configure<CaptchaOptions>(captchaOptions);

    #endregion

    return builder;
  }

  public static PureCodeApplicationBuilder UseUser(this PureCodeApplicationBuilder builder)
  {
    using var scope = builder.App.ApplicationServices.CreateScope();
    var profileManager = scope.ServiceProvider.GetService<ProfileManager>();
    profileManager?.InitializeAsync().Wait();
    return builder;
  }
}
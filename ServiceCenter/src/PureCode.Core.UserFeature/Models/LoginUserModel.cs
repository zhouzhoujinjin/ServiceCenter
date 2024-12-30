using System.Text.Json.Serialization;

namespace PureCode.Core.Models;

public class LoginUserModel
{
  /// <summary>
  /// 用户名
  /// </summary>
  [JsonPropertyName("userName")]
  public required string UserName { get; set; }

  /// <summary>
  /// 密码
  /// </summary>
  [JsonPropertyName("password")]
  public required string Password { get; set; }

  /// <summary>
  /// 记住登录
  /// </summary>
  [JsonPropertyName("rememberMe")]
  public bool? RememberMe { get; set; }


  /// <summary>
  /// 验证码
  /// </summary>
  [JsonPropertyName("captchaId")]
  public string? CaptchaId { get; set; }

  /// <summary>
  /// 验证码
  /// </summary>
  [JsonPropertyName("captchaCode")]
  public string? CaptchaCode { get; set; }
}
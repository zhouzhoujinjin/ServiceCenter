using PureCode.Utils.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PureCode.Core.Kernel
{
  public static class PermissionClaimNames
  {
    public const string ApiPermission = "api";
    public const string ActionPermission = "action";
    public const string RoutePermission = "route";
  }

  public static class JsonOptions
  {
    public static readonly JsonSerializerOptions JsonSerializerDefaultOptions = new()
    {
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      PropertyNameCaseInsensitive = true,
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
      DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    static JsonOptions()
    {
      JsonSerializerDefaultOptions.Converters.Add(new JsonStringEnumConverter(namingPolicy: JsonNamingPolicy.CamelCase));
      JsonSerializerDefaultOptions.Converters.Add(new DictionaryStringObjectJsonConverter());
    }
  }

  public static class SystemProfileKeyCategory
  {
    public const string Public = "public";
    public const string Brief = "brief";
    public const string Searchable = "searchable";
  }

  public static class ProfileKeys
  {
    public const string ClaimTypePrefix = "Profiles";
    public const string Surname = "surname";
    public const string GivenName = "givenName";
    public const string GenderCode = "genderCode";
    public const string GenderText = "genderText";
    public const string FullName = "fullName";
    public const string Avatar = "avatar";
    public const string IdPhoto = "idPhoto";
    public const string PinYin = "pinyin";
    public const string CreatorName = "creatorName";
    public const string CreatorId = "creatorId";
  }

  public static class DefaultProfileKeyClassTypes
  {
    public const string Bool = "bool";
    public const string String = "string";
    public const string Date = "date";
    public const string Number = "number";
  }
}
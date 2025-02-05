using Approval.Abstracts;
using Approval.Abstracts.Models;
using CyberStone.Core.Utils;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using Approval.Utils;

namespace Approval.Hooks
{
  public class LicenseRequestHook : IApprovalHook
  {
    private readonly LicenseServiceOptions options;

    public LicenseRequestHook(IOptions<LicenseServiceOptions> optionsAccessor)
    {
      this.options = optionsAccessor.Value;
    }
    public async Task<Dictionary<string, string>> ExecuteAsync(Dictionary<string, string> formData, Type dataType)
    {
      if (dataType == typeof(LicenseModel))
      {
        var licenseModel = (LicenseModel)DotSplittedKeyDictionaryToObjectConverter.Parse(formData, dataType);
        if (licenseModel != null)
        {
          var httpClient = new HttpClient();
          var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
          var sign = $"{options.SecretKey}:{licenseModel.MachineId}:{timestamp}".ComputeMd5().ToLower();
          var wrapperData = new Dictionary<string, object>
          {
            {"appId", options.AppId},
            {"timestamp",  timestamp},
            {"sign",  sign},
            {"info", new {
                machineId = licenseModel.MachineId,
                leftDays = licenseModel.LeftDays,
                features = licenseModel.Features,
              }
            }
          };
          var response = await httpClient.PostAsJsonAsync(options.ApiUrl, wrapperData);
          if (response != null && response.IsSuccessStatusCode)
          {
            var result = JsonSerializer.Deserialize<Dictionary<string, object>>(await response.Content.ReadAsStringAsync());
            licenseModel.LicenseCode = result["data"].ToString();
          }

          formData["licenseCode"] = licenseModel.LicenseCode;
        }
      }
      return formData;
    }
  }
}
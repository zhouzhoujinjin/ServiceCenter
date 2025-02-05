using Approval.Abstracts.Models;

namespace Approval.Abstracts
{
  public interface IApprovalHook
  {
    public Task<Dictionary<string, string>> ExecuteAsync(Dictionary<string, string> formData, Type dataType);
  }
}
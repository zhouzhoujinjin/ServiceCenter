using PureCode.Core.Models;

namespace Approval.Interface
{
  public interface IApprovalFlowNode
  {    
    public ulong UserId { get; }
    public int ItemId { get; }
    public ApprovalFlowNodeType NodeType { get; set; }
    public ApprovalActionType ActionType { get; set; }
    public Dictionary<ApprovalActionType, List<string>> Hooks {  get; set; }
    public IApprovalFlowNode Previous { get; set; }
    public IApprovalFlowNode Next { get; set; }
    public UserModel User { get; set; }

  }


  public interface ILogicApprovalFlowNode: IApprovalFlowNode
  {
    public ICollection<IApprovalFlowNode> Children { get; set; }

  }
}
 
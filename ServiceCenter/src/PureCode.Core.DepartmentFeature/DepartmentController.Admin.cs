using Microsoft.AspNetCore.Mvc;
using PureCode.Utils;

namespace PureCode.Core.DepartmentFeature
{

  public partial class DepartmentController : ControllerBase
  {
    [HttpGet("/api/admin/departments", Name = "管理员 - 部门树")]
    public async Task<AjaxResp<ICollection<DepartmentModel>?>> TreeAsync()
    {
      var departmentTree = await departmentManager.GetDepartmentTreeAsync();
      return new AjaxResp<ICollection<DepartmentModel>?> { Data = departmentTree };
    }

    [HttpGet("/api/admin/departments/{depId}", Name = "管理员 - 部门用户列表")]
    public async Task<AjaxResp<DepartmentModel>?> DepartmentUsersAsync(ulong depId)
    {
      var department = await departmentManager.GetDepartmentWithUsersAsync(depId);
      return new AjaxResp<DepartmentModel> { Data = department };
    }

    [HttpGet("/api/admin/departments/users/{userId}", Name = "管理员 - 用户部门列表")]
    public async Task<AjaxResp<ICollection<UserDepartment>>?> UserDepartmentsAsync(ulong userId)
    {
      var departments = await departmentManager.GetUserDepartments(userId);
      return new AjaxResp<ICollection<UserDepartment>> { Data = departments.ToList() };
    }

    [HttpPost("/api/admin/departments", Name = "管理员 - 创建部门")]
    public async Task<AjaxResp> CreateAsync([FromBody] DepartmentModel department)
    {
      var currentUserId = HttpContext.GetUserId();
      await departmentManager.AddDepartmentAsync(department, currentUserId);
      return new AjaxResp { Message = "创建部门成功" };
    }

    [HttpPost("api/admin/departments/{departmentId}/update", Name = "管理员 - 更新部门及部门人员信息")]
    public async Task<AjaxResp> UpdateAsync(ulong departmentId, [FromBody] DepartmentModel department)
    {
      await departmentManager.UpdateDepartmentAsync(departmentId, department);
      await departmentManager.UpdateDepartmentUserAsync(departmentId, department);
      return new AjaxResp { Message = "更新部门信息成功" };
    }

    [HttpPost("api/admin/departments/{departmentId}/parent", Name = "管理员 - 更新部门结构")]
    public async Task<AjaxResp> UpdateAsync(ulong departmentId, [FromBody] ulong? parentDepartmentId)
    {
      await departmentManager.UpdateHierarchicalAsync(departmentId, parentDepartmentId);
      return new AjaxResp { };
    }

    [HttpPost("api/admin/departments/{deptId}/delete", Name = "管理端 - 删除部门")]
    public async Task<AjaxResp> DeleteAsync(ulong deptId)
    {
      await departmentManager.DeleteDepartmentAsync(deptId);
      return new AjaxResp { Message = "删除成功" };
    }



  }
}

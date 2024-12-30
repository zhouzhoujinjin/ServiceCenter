﻿using Approval.Entities;
using Approval.Models;
using Microsoft.EntityFrameworkCore;
using PureCode.Core.DepartmentFeature;
using PureCode.Core.Models;

namespace Approval.Managers
{
  public class TemplateManager
  {
    private readonly ApprovalDbContext context;
    private readonly DepartmentManager departmentManager;

    public TemplateManager(ApprovalDbContext context, DepartmentManager departmentManager)
    {
      this.context = context;
      this.departmentManager = departmentManager;
    }

    public async Task<IEnumerable<ApprovalTemplateEntity>> GetAllTemplates()
    {
      return await context.ApprovalTemplates.ToListAsync();
    }

    public async Task<IEnumerable<ApprovalTemplateEntity>> GetUserTemplates(UserModel user)
    {
      var templates = await context.ApprovalTemplates.Where(x => x.IsVisible).ToListAsync();
      if (user == null)
      {
        return templates;
      }
      var templateList = new List<ApprovalTemplateEntity>();

      foreach (var template in templates)
      {
        if (template.Applicants == null)
        {
          templateList.Add(template);
          continue;
        }
        var userCount = template.Applicants.Users.Count();
        var deptCount = template.Applicants.Departments.Count();

        if (userCount == 0 && deptCount == 0)
        {
          templateList.Add(template);
          continue;
        }
        else
        {
          if (userCount != 0)
          {
            var isExistUser = template.Applicants.Users.Any(x => x.Id == user.Id);
            if (isExistUser)
            {
              templateList.Add(template);
              continue;
            }
          }
          if (deptCount != 0)
          {
            if (user.Profiles.ContainsKey("departmentIds"))
            {
              var departmentIds = user.Profiles["departmentIds"].ToString().Split(",");
              var isExist = false;
              foreach (string id in departmentIds)
              {
                var allowDeptApply = template.Applicants.Departments.Any(x => x.Id == ulong.Parse(id));
                if (allowDeptApply)
                {
                  isExist = true;
                  break;
                }
              }
              if (isExist)
              {
                templateList.Add(template);
                continue;
              }
            }
          }
        }
      }
      return templateList;
    }

    public async Task<ApprovalTemplateEntity> GetTemplateByName(string templateName)
    {
      return await context.ApprovalTemplates.FirstOrDefaultAsync(x => x.Name == templateName);
    }

    /// <summary>
    /// 获取用户待审批及已处理的模板
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<IEnumerable<ApprovalTemplateEntity>> GetUserApprovalTemplates(UserModel user)
    {
      var approvals = await context.ApprovalNodes.Include(x => x.Item).ThenInclude(x => x.Template).Where(x => x.UserId == user.Id
        && (x.ActionType == ApprovalActionType.Approved || x.ActionType == ApprovalActionType.Pending || x.ActionType == ApprovalActionType.Rejected)
        && x.NodeType != ApprovalFlowNodeType.Start
        && x.Item.Status != ApprovalItemStatus.Draft).ToListAsync();
      var templates = approvals.Select(x => x.Item.Template).Distinct();
      return templates;
    }

    /// <summary>
    /// 获得用户抄送的模板
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<IEnumerable<ApprovalTemplateEntity>> GetUserCcTemplates(UserModel user)
    {
      var approvals = await context.ApprovalNodes.Include(x => x.Item).ThenInclude(x => x.Template).Where(x => x.UserId == user.Id
         && (x.ActionType == ApprovalActionType.Approved)
         && x.NodeType != ApprovalFlowNodeType.Cc
         && x.Item.Status != ApprovalItemStatus.Draft).ToListAsync();
      var templates = approvals.Select(x => x.Item.Template).Distinct();
      return templates;
    }

    public async Task<ApprovalTemplateEntity> GetTemplateById(long id)
    {
      return await context.ApprovalTemplates.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateTemplateWorkflow(string name, Workflow workflow)
    {
      var entity = await context.ApprovalTemplates.FirstOrDefaultAsync(x => x.Name == name);
      entity.LastUpdatedTime = DateTime.Now;
      entity.Workflow = workflow;
      context.ApprovalTemplates.Update(entity);
      await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ulong>> GetDepartmentIdsAsync(ulong userId, string templateName)
    {
      var departments = await departmentManager.GetUserDepartments(userId);
      var departmentIds = departments.Select(x => x.DepartmentId).ToList();
      var templates = await context.ApprovalTemplates.Where(x => x.Name.Equals(templateName)).Select(y => y.DepartmentIds).FirstOrDefaultAsync();
      if (templates == null || templates.Count() == 0) return null;
      var intersectedList = departmentIds.Intersect(templates);
      if (intersectedList == null || intersectedList.Count() == 0) return null;
      return intersectedList;
    }


  }
}

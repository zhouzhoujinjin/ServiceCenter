import request from '../../../utils/request'
import { API } from '../../../config'

export const GetTemplateGroup = async (action) => {
  const data = await request.get(`${API}/valueSpaces/approvalTemplateGroup`)
  action(data.conditions)
}

export const GetTemplateNames = async (action) => {
  const data = await request.get(`${API}/approval/templates`)
  action && action(data)
}

export const GetApprovalTemplates = async (action) => {
  const data = await request.get(`${API}/approval/approvalTemplates`)
  action && action(data)
}

export const GetApprovalCcTemplates = async (action) => {
  const data = await request.get(`${API}/approval/approvalCcTemplates`)
  action && action(data)
}

export const GetTemplate = async (name, action) => {
  const data = await request.get(`${API}/approval/templates/${name}`)
  action && action(data)
}

export const GetDeptUsers = async (action) => {
  const data = await request.get(`${API}/approval/dept-users`)
  action && action(data)
}

export const GetDepartmentIds = async (templateId, action) => {
  const result = await request.get(`${API}/approval/${templateId}/departmentIds`)
  return action ? action(result) : result
}

export const GetTransUsers = async (itemId, action) => {
  const data = await request.get(`${API}/approval/dept-users/${itemId}`)
  action && action(data)
}

export const GetDepartments = async (action) => {
  const data = await request.get(`${API}/approval/account-departments`)
  action && action(data)
}

export const CreateApprovalItem = async (templateName, data, submitType, action) => {
  const result = await request.post(`${API}/approval/${templateName}/${submitType}`, data)
  return action ? action(result) : result
}

export const GetApprovalItems = async ({ filters, page, size }, action) => {
  const query = JSON.stringify(filters)
  const data = await request.get(`${API}/approval/items`, { params: { query, page, size } })
  action && action(data)
}

export const GetApprovalItem = async (itemId, action) => {
  const data = await request.get(`${API}/approval/items/${itemId}`)
  action && action(data)
}

export const UpdateApprovalItem = async (templateName, itemId, values, submitType, action) => {
  const data = await request.put(`${API}/approval/${templateName}/${itemId}/${submitType}`, values)
  return action ? action(data) : data
}

export const DeleteApprovalItem = async (itemId, action) => {
  const data = await request.delete(`${API}/approval/items/${itemId}`)
  return action ? action(data) : data
}

export const DeleteFinalFile = async (itemId, title, action) => {
  const data = await request.delete(`${API}/approval/final/${itemId}/remove/${title}`)
  return action ? action(data) : data
}

export const UpdateApproval = async ({ itemId, nodeId, approval }, action) => {
  const result = await request.put(`${API}/approval/${itemId}/nodes/${nodeId}/update`, approval)
  return action ? action(result) : result
}

export const UpdateSelfApproval = async ({ itemId, nodes }, action) => {
  const cleanedList = nodes.map((x) => {
    const n = { ...x }
    if (n.children && n.children.length) {
      n.children = n.children.map((y) => {
        const nn = { ...y }
        if (typeof nn.id === 'string') {
          nn.id = 0
        }
        return nn
      })
    }
    if (typeof n.id === 'string') {
      n.id = 0
    }
    return n
  })

  const result = await request.put(`${API}/approval/${itemId}/nodes/self/next`, cleanedList)
  return action ? action(result) : result
}

export const GetUserApprovalCounts = async (action) => {
  const data = await request.get(`${API}/approval/counts`)
  action && action(data)
}

export const GetUserCcCounts = async (action) => {
  const data = await request.get(`${API}/approval/cc-counts`)
  action && action(data)
}

export const GetApprovals = async ({ filters, page, size }, action) => {
  const query = JSON.stringify(filters)
  const data = await request.get(`${API}/approval/approvals`, { params: { query, page, size } })
  action && action(data)
}

export const GetApprovalUsers = async ({ query, actionType, templateName, startDate, endDate, itemId }, action) => {
  const filters = {}
  if (query) filters.query = query
  if (templateName) filters.templateName = templateName
  if (startDate) filters.startDate = startDate
  if (endDate) filters.endDate = endDate
  if (actionType) filters.actionType = actionType
  if (itemId) filters.itemId = itemId

  const data = await request.get('/api/approval/users', {
    params: { query: JSON.stringify(filters) }
  })
  return action ? action(data) : data
}

export const GetTemplateAll = async (action) => {
  const data = await request.get(`${API}/approval/templateNames`)
  action && action(data)
}

export const GetApprovalItemsAdmin = async ({ filters, page, size }, action) => {
  const query = JSON.stringify(filters)
  const data = await request.get(`${API}/approval/items/admin/all`, { params: { query, page, size } })
  action && action(data)
}

export const GetLeaveItems = async ({ filters, page, size }, action) => {
  const query = JSON.stringify(filters)
  const data = await request.get(`${API}/approval/items/leave/all`, { params: { query, page, size } })
  action && action(data)
}

export const ExportLeave = async (filters, action) => {
  const query = JSON.stringify(filters)
  const data = await request.get(`${API}/approval/items/leave/export`, { params: { query } })
  action && action(data)
}

export const GetOvertimeItems = async ({ filters, page, size }, action) => {
  const query = JSON.stringify(filters)
  const data = await request.get(`${API}/approval/items/overtime/all`, { params: { query, page, size } })
  action && action(data)
}

export const UpdateOvertimeFinishDate = async (model, action) => {
  const data = await request.put(`${API}/approval/items/overtime/update`, model)
  action && action(data)
}

export const ExportOvertime = async (filters, action) => {
  const query = JSON.stringify(filters)
  const data = await request.get(`${API}/approval/items/overtime/export`, { params: { query } })
  action && action(data)
}

export const PreviewFlow = async (templateName, data, action) => {
  const result = await request.post(`${API}/approval/${templateName}`, data)
  return action ? action(result) : result
}

export const TransUser = async (nodeId, trans, action) => {
  const data = await request.post(`${API}/approval/trans/${nodeId}`, trans)
  action && action(data)
}

export const ExportPdf = async (itemId, action) => {
  const data = await request.get(`${API}/approval/pdf/${itemId}`)
  action && action(data)
}

export const GetApprovalVerifyItems = async ({ filters, page, size }, action) => {
  const query = JSON.stringify(filters)
  const data = await request.get(`${API}/approval/items/verify`, { params: { query, page, size } })
  action && action(data)
}

export const PublishVerifiedItem = async ({ itemId, purview }, action) => {
  const result = await request.put(`${API}/approval/items/${itemId}/publish`, purview)
  return action ? action(result) : result
}

export const DeleteVerifiedFile = async (itemId, title, action) => {
  const data = await request.delete(`${API}/approval/verify/${itemId}/remove/${title}`)
  return action ? action(data) : data
}

export const GetPublishType = async (action) => {
  const data = await request.get(`${API}/valueSpaces/publishType`)
  action(data.conditions)
}

export const PublishNotice = async (action) => {
  const data = await request.get(`${API}/approval/items/notice`)
  action && action(data)
}

export const IsUpdate = async (itemId, nodeId, comment, action) => {
  const result = await request.put(`${API}/approval/items/${itemId}/${nodeId}/isupdate`, comment)
  return action ? action(result) : result
}

export const CreateCopyItem = async (itemId, action) => {
  const data = await request.post(`${API}/approval/items/copy`, itemId)
  action && action(data)
}

export const RecallItem = async (itemId, action) => {
  const result = await request.put(`${API}/approval/items/${itemId}/recall`)
  return action ? action(result) : result
}

export const PressItem = async (itemId, action) => {
  const result = await request.get(`${API}/approval/items/${itemId}/press`)
  return action ? action(result) : result
}

export const UpdateBackTime = async (itemId, backTime, action) => {
  const result = await request.put(`${API}/approval/items/${itemId}/backtime`, backTime)
  return action ? action(result) : result
}

export const IsLastApproval = async (itemId, action) => {
  const result = await request.get(`${API}/approval/items/${itemId}/lastApporval`)
  return action ? action(result) : result
}

export const ApprovalExport = async (filters, action) => {
  const query = JSON.stringify(filters)
  const data = await request.get(`${API}/approval/approval-export`, { params: { query } })
  action && action(data)
}
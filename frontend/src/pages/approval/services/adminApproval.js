import request from '../../../utils/request'
import { API } from '../../../config'

export const GetTemplates = async (action) => {
  const data = await request.get(`${API}/admin/approval/templates`)
  action && action(data)
}

export const GetTemplate = async (name, action) => {
  if (name) {
    const data = await request.get(`${API}/admin/approval/templates/${name}`)
    action && action(data)
  }
}

export const UpdateTemplate = async (name, workflow, action) => {
  if (name) {
    const data = await request.put(`${API}/admin/approval/templates/${name}`, workflow)
    action && action(data)
  }
}

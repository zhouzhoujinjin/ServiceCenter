import { RequestCallback } from "~/shared";
import { PagedAjaxResp, api } from "~/shared/request";
import { Permission, Role } from "../types";
import { UserInfo } from "~/types";

const API: string = import.meta.env.VITE_API_HOST;

export const GetRoles = async (
  action?: RequestCallback<PagedAjaxResp<Role>>
) => {
  const data = await api.get<Role[]>(`${API}/admin/roles`);
  action && action(data as PagedAjaxResp<Role>);
};

export const GetPermissions = async (
  action?: RequestCallback<Permission[]>
) => {
  const data = await api.get<Permission[]>(`${API}/admin/permissions`);
  action && action(data as Permission[]);
};

export const GetRole = async (
  roleName: string,
  action?: RequestCallback<Role>
) => {
  const data = await api.get<Role>(`${API}/admin/roles/${roleName}`);
  action && action(data as Role);
};

export const GetRoleWithoutClaims = async (
  roleName: string,
  action?: RequestCallback<Role>
) => {
  const data = await api.get<Role>(
    `${API}/admin/roles/${roleName}/withoutClaims`
  );
  action && action(data as Role);
};

export const GetUserList = async (action?: RequestCallback<UserInfo[]>) => {
  const data = await api.get<UserInfo[]>(`${API}/departments/users`);
  action && action(data as UserInfo[]);
};

export const CreateRole = async (role: Role, action?: () => void) => {
  await api.post<undefined>(`${API}/admin/roles`, role);
  action && action();
};

export const UpdateRole = async (
  roleName: string,
  role: Role,
  action?: () => void
) => {
  await api.post<undefined>(`${API}/admin/roles/${roleName}/update`, role);
  action && action();
};

export const UpdateRoleUsers = async (
  roleName: string,
  users: UserInfo[],
  action?: () => void
) => {
  await api.post<undefined>(`${API}/admin/roles/${roleName}/users`, users);
  action && action();
};

export const RemoveRole = async (roleName: string, action?: () => void) => {
  await api.post<undefined>(`${API}/admin/roles/${roleName}/delete`, undefined);
  action && action();
};

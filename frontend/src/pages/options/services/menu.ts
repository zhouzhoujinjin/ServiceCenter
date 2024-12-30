import { RequestCallback } from "~/shared";
import { MenuItem } from "~/types";
import { api } from "~/shared/request";

const API: string = import.meta.env.VITE_API_HOST;

export const GetAdminMenu = async (action?: RequestCallback<MenuItem[]>) => {
  const data = await api.get<MenuItem[]>(`${API}/admin/nav`);
  action && action(data as MenuItem[]);
};

export const CreateMenu = async (menuItem: MenuItem, action?: () => void) => {
  await api.post<MenuItem>(`${API}/admin/nav/create`, menuItem);
  action && action();
};

export const UpdateMenu = async (menuItem: MenuItem, action?: () => void) => {
  await api.post<MenuItem>(`${API}/admin/nav/update`, menuItem);
  action && action();
};

export const UpdateMenuStruct = async (
  menutId: number,
  parentMenuId?: number
) => {
  await api.post<undefined>(
    `${API}/admin/nav/${menutId}/parent`,
    parentMenuId || null
  );
};

export const RemoveMenu = async (
  menuId: number,
  action?: () => void
) => {
  await api.post<undefined>(
    `${API}/admin/nav/${menuId}/delete`,
    undefined
  );
  action && action();
};
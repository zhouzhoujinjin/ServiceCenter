import { RequestCallback } from "~/shared";
import { SystemConfig } from "~/types";
import { api } from "~/shared/request";

const API: string = import.meta.env.VITE_API_HOST;

export const GetSystemConfig = async (
  action?: RequestCallback<SystemConfig>
) => {
  const data = await api.get<SystemConfig>(`${API}/config`);
  action && action(data as SystemConfig);
};

export const UpdateSystemConfig = async (
  systemConfig: SystemConfig,
  action?: () => void
) => {
  await api.post<SystemConfig>(`${API}/admin/config/update`, systemConfig);
  action && action();
};

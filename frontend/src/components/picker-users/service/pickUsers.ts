import { UserInfo } from "~/types";
import { api, RequestCallback } from "~/shared";

const API: string = import.meta.env.VITE_API_HOST;

export const GetUserList = async (action?: RequestCallback<UserInfo[]>) => {
  const data = await api.get<UserInfo[]>(`${API}/departments/users`);
  action && action(data as UserInfo[]);
};

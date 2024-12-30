import { RequestCallback } from "~/shared";
import { PagedAjaxResp, api } from "~/shared/request";
import { UserData } from "../types";

const API: string = import.meta.env.VITE_API_HOST;

export const GetAdminUsers = async (
  {
    page,
    pageSize,
    filters,
  }: { page?: number; pageSize?: number; filters?: Record<string, string> },
  action?: RequestCallback<PagedAjaxResp<UserData>>
) => {
  const searchParams: any = {
    page: page || 0,
    size: pageSize,
  };
  if (filters && filters.query) {
    searchParams.query = filters.query;
  }
  const data = await api.get<PagedAjaxResp<UserData>>(`${API}/admin/users`, {
    searchParams: searchParams,
  });

  if (data?.data) {
    (data?.data as UserData[]).forEach((element) => {
      if (element.profiles && element.profiles.birthday) {
        element.profiles.birthday = new Date(element.profiles.birthday);
      }
    });
  }
  action && action(data as PagedAjaxResp<UserData>);
};

export const CreateUser = async (
  userData: UserData,
  action?: (response: any) => void
) => {
  const resp = await api.post<undefined>(`${API}/admin/users`, userData);
  action && action(resp);
};

export const UpdateAdminUser = async (
  userName: string,
  userData: UserData,
  action?: (response: any) => void
) => {
  const resp = await api.post<undefined>(
    `${API}/admin/users/${userName}`,
    userData
  );
  action && action(resp);
};

export const RemoveUser = async (userName: string, action?: () => void) => {
  await api.post<undefined>(`${API}/admin/users/${userName}/delete`, undefined);
  action && action();
};

export const Pinyin = async (
  chinese: string,
  action?: (response: string) => void
) => {
  const resp = await api.get<string>(`${API}/tools/pinyin/${chinese}`);
  action && action(resp as string);
};

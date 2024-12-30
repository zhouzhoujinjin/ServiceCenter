import { RequestCallback } from "~/shared";
import { api } from "~/shared/request";
import { ValueSpacesItem, ValueSpacesProps } from "../types";

const API: string = import.meta.env.VITE_API_HOST;

export const GetValueSpaces = async (
  action?: RequestCallback<ValueSpacesItem>
) => {
  const data = await api.get<ValueSpacesItem>(`${API}/valuespaces`);
  action && action(data as ValueSpacesItem);
};

export const GetValueSpaceInfo = async (
  vsName: string,
  action?: RequestCallback<ValueSpacesProps>
) => {
  const data = await api.get<ValueSpacesProps>(`${API}/valuespaces/${vsName}`);
  action && action(data as ValueSpacesProps);
};

export const UpdateValueSpace = async (
  vs: ValueSpacesProps,
  action?: () => void
) => {
  await api.post<undefined>(`${API}/admin/valueSpaces/${vs.name}/update`, vs);
  action && action();
};

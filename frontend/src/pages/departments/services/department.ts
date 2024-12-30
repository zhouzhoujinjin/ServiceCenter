import { Department } from "../types";
import { api, RequestCallback } from "~/shared";

const API: string = import.meta.env.VITE_API_HOST;

export const GetDepartmentTree = async (
  action?: RequestCallback<Department[]>
) => {
  const data = await api.get<Department[]>(`${API}/admin/departments`);
  action && action(data as Department[]);
};

export const GetDepartmentUsers = async (
  departmentId: number,
  action?: RequestCallback<Department>
) => {
  const data = await api.get<Department[]>(
    `${API}/admin/departments/${departmentId}`
  );
  action && action(data as unknown as Department);
};

export const CreateDepartment = async (
  department: Department,
  action?: () => void
) => {
  await api.post<undefined>(`${API}/admin/departments`, department);
  action && action();
};

export const UpdateDepartment = async (
  departmentId: number,
  department: Department,
  action?:()=>void
) => {
  await api.post<undefined>(
    `${API}/admin/departments/${departmentId}/update`,
    department
  );
  action && action();
};

export const UpdateDepartmentStruct = async (
  departmentId: number,
  parentDepartmentId?: number
) => {
  await api.post<undefined>(
    `${API}/admin/departments/${departmentId}/parent`,
    parentDepartmentId || null
  );
};

export const RemoveDepartment = async (
  departmentId: number,
  action?: () => void
) => {
  await api.post<undefined>(
    `${API}/admin/departments/${departmentId}/delete`,
    undefined
  );
  action && action();
};

import { api } from "~/shared/request";

const API: string = import.meta.env.VITE_API_HOST;

export const Prompt = async (
  askContent: string,
  action?: (answer: any) => void
) => {
  const rsp = await api.post<undefined>(`${API}/ai/ask`, askContent);
  action && action(rsp);
};

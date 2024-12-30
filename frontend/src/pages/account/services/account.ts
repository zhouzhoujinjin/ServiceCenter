import { api } from "~/shared/request";
import { CaptchaResp, LoginParams, LoginResp } from "../types";

const API = import.meta.env.VITE_API_HOST;

export const FetchCaptchaImage = async (
  action: (data: CaptchaResp) => void
) => {
  const data = await api.get<CaptchaResp>(`${API}/account/captcha`);
  action && action(data as CaptchaResp);
  return data;
};

export const Login = async (
  { userName, password, captchaCode, captchaId, rememberMe }: LoginParams,
  action: (data: LoginResp) => void
) => {
  const data = await api.post<LoginResp>(`${API}/account/login`, {
    userName,
    password,
    captchaCode,
    captchaId,
    rememberMe,
  });
  action && action(data as LoginResp);
};

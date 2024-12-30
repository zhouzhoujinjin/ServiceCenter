export type LoginParams = {
  userName: string;
  password: string;
  captchaCode?: string;
  captchaId?: string;
  rememberMe?: boolean
};

export type CaptchaResp = {
  id: string;
  content: string;
};

export type LoginResp = {
  accessToken: string;
  refreshToken?: string;
};

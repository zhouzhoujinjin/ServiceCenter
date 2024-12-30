import { RequestCallback, SystemConfigType } from "../types";

export const GetConfig = async (action?: RequestCallback<SystemConfigType>) => {
  action && action();
};

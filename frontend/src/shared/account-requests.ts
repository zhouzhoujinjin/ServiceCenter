import { MenuItem, UserInfo, UserProfile } from "~/types";
import { RequestCallback, api } from "./request";

const API: string = import.meta.env.VITE_API_HOST;

const NAV_KEY = "CURRENT_NAV";
const OPENED_TABS_KEY = "OPENED_TABS";
const ACTIVE_TAB_KEY = "ACTIVE_TAB";
const PERMISSIONS_KEY = "PERMISSIONS";
const PROFILES_KEY = "PROFILES";

/**
 * Fetches user profile information from the API.
 */

export const GetAccountMenu = async (action?: RequestCallback<MenuItem[]>) => {
  const cached = sessionStorage.getItem(NAV_KEY);
  if (cached) {
    action && action(JSON.parse(cached));
  } else {
    const data = await api.get<MenuItem[]>(`${API}/account/nav`);
    if (data) {
      sessionStorage.setItem(NAV_KEY, JSON.stringify(data));
      action && action(data as MenuItem[]);
    }
  }
};

/**
 * Fetches user profile information from the API.
 *
 * @param action - Callback to handle profile data response
 * @param errorHandler - Callback to handle errors
 */
export const GetAccountProfile = async (
  action?: RequestCallback<UserInfo>,
  errorHandler?: OnErrorEventHandler
) => {
  try {
    const data = await api.get<UserInfo>(`${API}/account/profile`);
    // console.log(data)
    action && action(data as UserInfo);
  } catch (e: any) {
    errorHandler && errorHandler(e);
  }
};

/**
 * Fetches user permissions from the API.
 *
 * @param action - Callback to handle permissions response
 * @param errorHandler - Callback to handle errors
 */

export const GetAccountPermissions = async (
  action?: RequestCallback<string[]>,
  errorHandler?: OnErrorEventHandler
) => {
  try {
    action && action();
  } catch (e: any) {
    errorHandler && errorHandler(e);
  }
};

export const ClearAccountCache = () => {
  window.sessionStorage.removeItem(PERMISSIONS_KEY);
  window.sessionStorage.removeItem(PROFILES_KEY);
  window.sessionStorage.removeItem(NAV_KEY);
  window.sessionStorage.removeItem(OPENED_TABS_KEY);
  window.sessionStorage.removeItem(ACTIVE_TAB_KEY);
};

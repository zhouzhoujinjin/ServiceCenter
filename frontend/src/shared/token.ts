import { jwtDecode } from "jwt-decode";
import { UserInfo } from "~/types";

export const STORAGE_TOKEN_NAME = "TOKEN";
export const STORAGE_CURRENT_USER = "CURRENT_USER";

let user: UserInfo | null = null;

export const token = {
  get() {
    return (
      localStorage.getItem(STORAGE_TOKEN_NAME) ||
      sessionStorage.getItem(STORAGE_TOKEN_NAME)
    );
  },
  getUser() {
    if (!user) {
      const userJson = localStorage.getItem(STORAGE_CURRENT_USER);
      if (userJson) {
        user = JSON.parse(userJson);
      }
    }
    return user;
  },
  save(token: string) {
    localStorage.setItem(
      STORAGE_CURRENT_USER,
      JSON.stringify(jwtDecode(token))
    );
    localStorage.setItem(STORAGE_TOKEN_NAME, token);
  },
  remove() {
    localStorage.removeItem(STORAGE_TOKEN_NAME);
    localStorage.removeItem(STORAGE_CURRENT_USER);
    user = null;
  },
};

import { UserInfo, UserProfile } from "~/types";

export type UserData = {
  id?: number;
  userName: string;
  isVisible: boolean;
} & UserProfile &
  UserRole;

export type UserRole = {
  roles?: { [key: string]: any };
};

export type Permission = {
  name: string;
  value: string;
  group: string;
};

export type Role = {
  name?: string;
  title?: string;
  users?: UserInfo[];
  claims?: string[];
};

export interface Department {
  id: number;
  title: string;
  children?: Department[];
  users?: DepartmentUser[];
}

export interface UserProfile {
  userId?: number;
  fullName?: string;
  birthday?: Date;
  avatar?: string;
}

export interface DepartmentUser {
  id?:number;
  userId?: number;
  position?: string;
  level?: number;
  isUserMajorDepartment?: boolean;
  userName?: string;
  profiles?: UserProfile;
}
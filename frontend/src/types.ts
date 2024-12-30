export type SystemConfigType = Record<string, any>;

export type WithChildren<T> = T & { children?: WithChildren<T>[] };
export type WithParent<T> = T & { parent?: WithParent<T> };
export type WithParents<T> = T & { parents?: WithParents<T>[] };

export interface MenuItem {
  title: string;
  uri: string;
  childrenInvisible?: boolean;
  invisible?: boolean;
  iconType?: IconType;
  icon?: string;
  type?: "route" | "link" | "group";
  isBlank?: boolean;
  children?: MenuItem[];
}

export type IconType = "builtin" | "image" | undefined;

export type FullMenuProps = {
  items: MenuItem[];
  variant?: "nav" | "collapsed-nav" | "button";
  activeKey: string;
  onChange?: (key: string) => void;
};

export type UserProfile = {
  profiles?: { [key: string]: any };
};

export type UserInfo = {
  id?: number;
  userId?: number;
  userName: string;
  fullName?: string;
  avatar?: string;
} & UserProfile;

export type AccountInfos = {
  user?: UserInfo;
  status: "pending" | "success" | "error";
  permissions?: string[];
  nav?: any[];
} & UserProfile;

export type ValueLabelInfo = {
  value: any;
  label: string;
  info?: any;
};

export type SystemConfig = {
  openStartDate: Date;
  openEndDate: Date;
  siteName?:string;
  siteRoot?:string;
  logoUri?:string;
  defaultAvatarSize?:Size;
  siteUnderMaintenance?:string;
  beiAnNO?:string;
  SiteUnderMaintenance?:boolean
};

export type Size={
  width:number,
  height:number,
}
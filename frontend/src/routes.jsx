import {
  IndexPage,
  ProfilePage,
  UsersPage,
  UserPage,
  RolesPage,
  RolePage,
  ConfigPage as SystemConfigPage,
  MenuPage,
  ValueSpacePage,
  ValueSpacesPage,
  DepartmentsPage,
  ApprovalEditPage,
  ApprovalsPage,
  ApprovalInfoPage,
  ApprovalsWaitPage,
  ApprovalsDisposePage,
  ApprovalsSendPage,
  ApprovalsAdminPage,
  ApprovalVerifyPage,
  ApprovalTemplatesPage,
  ApprovalDesignPage,
} from "./pages"


export const COMPONENTS = {
  Index: IndexPage,
  Profile: ProfilePage,

  // 用户
  Users: UsersPage,
  User: UserPage,
  Roles: RolesPage,
  Role: RolePage,
  Departments: DepartmentsPage,

  // 系统
  SystemConfig: SystemConfigPage,
  Menu: MenuPage,
  ValueSpaces: ValueSpacesPage,
  ValueSpace: ValueSpacePage,


  //oa审批
  ApprovalEdit: ApprovalEditPage,
  ApprovalInfo: ApprovalInfoPage,
  Approvals: ApprovalsPage,
  ApprovalsWait: ApprovalsWaitPage,
  ApprovalsDispose: ApprovalsDisposePage,
  ApprovalsSend: ApprovalsSendPage,
  ApprovalsAdmin: ApprovalsAdminPage,
  ApprovalVerify: ApprovalVerifyPage,
  ApprovalsTemplates: ApprovalTemplatesPage,
  ApprovalDesign: ApprovalDesignPage,

}

export const paths = [
  { path: "/index", title: "首页", component: "Index" },
  {
    path: "/account/profile",
    exact: true,
    title: "个人设置",
    component: "Profile",
  },
  {
    path: "/departments",
    exact: true,
    title: "部门列表",
    component: "Departments",
  },
  {
    path: "/users/:id",
    title: "用户表单",
    component: "User",
  },
  {
    path: "/users",
    title: "用户列表",
    component: "Users",
  },
  {
    path: "/roles/:id",
    title: "角色表单",
    component: "Role",
  },
  {
    path: "/roles",
    title: "角色列表",
    component: "Roles",
  },
  {
    path: "/system/config",
    exact: true,
    title: "系统设置",
    component: "SystemConfig",
  },
  {
    path: "/system/menu",
    title: "菜单设置",
    component: "Menu",
  },
  {
    path: "/system/valueSpaces",
    exact: true,
    title: "字典列表",
    component: "ValueSpaces",
  },
  {
    path: "/system/valueSpaces/:name",
    exact: true,
    title: "字典表单",
    component: "ValueSpace",
  },
  //
  {
    path: "/approvals/all",
    exact: true,
    title: "审批管理",
    component: "Approvals",
  },
  {
    path: "/approvals",
    exact: true,
    title: "审批管理",
    component: "Approvals",
  },
  {
    path: "/approvals/:name/:id/edit",
    exact: true,
    title: "编辑审批",
    component: "ApprovalEdit",
  },
  {
    path: "/approvals/:id/info",
    exact: true,
    title: "流转审批",
    component: "ApprovalInfo",
  },
  {
    path: "/approvals/wait",
    exact: true,
    title: "我的待办",
    component: "ApprovalsWait",
  },
  {
    path: "/approvals/templates",
    exact: true,
    title: "流程设计列表",
    component: "ApprovalsTemplates",
  },
  {
    path: "/approvals/templates/:templateName/design",
    exact: true,
    title: "流程设计",
    component: "ApprovalDesign",
  },
  {
    path: "/approvals/dispose",
    exact: true,
    title: "我的已办",
    component: "ApprovalsDispose",
  },
  {
    path: "/approvals/send",
    exact: true,
    title: "抄送我",
    component: "ApprovalsSend",
  },
  {
    path: "/approvals/admin",
    exact: true,
    title: "全部审批",
    component: "ApprovalsAdmin",
  },
  {
    path: "/approvals/verify",
    exact: true,
    title: "校稿",
    component: "ApprovalVerify",
  },


]

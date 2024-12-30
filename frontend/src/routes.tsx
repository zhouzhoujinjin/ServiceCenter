import { DemoPage } from "./pages";
import { DepartmentPage } from "./pages/departments/DepartmentPage";
import { Home } from "./pages/home/Home";
import { ConfigPage } from "./pages/options/ConfigPage";
import { MenuPage } from "./pages/options/MenuPage";
import { RoleInfo } from "./pages/users/RoleInfo";

// import { RoleInfo } from "./pages/users/RoleInfo";
import { RolePage } from "./pages/users/RolePage";
import { RoleUsers } from "./pages/users/RoleUsers";
import { UserPage } from "./pages/users/UserPage";
import { ValueSpaceInfo } from "./pages/valueSpaces/ValueSpaceInfo";
import { ValueSpacePage } from "./pages/valueSpaces/ValueSpacePage";

export const routes = [
  {
    path: "/home",
    title: "首页",
    element: <Home />,
  },
  {
    path: "/account/profile",
    title: "个人设置",
    element: <DemoPage />,
  },
  {
    path: "/departments",
    title: "部门列表",
    element: <DepartmentPage />,
  },
  {
    path: "/users/index",
    title: "用户列表",
    element: <UserPage />,
  },
  {
    path: "/users/:id",
    title: "用户表单",
    element: <DemoPage />,
  },
  {
    path: "/roles/index",
    title: "角色列表",
    element: <RolePage />,
  },
  {
    path: "/roles/:name/users",
    title: "角色用户",
    element: <RoleUsers />,
  },
  {
    path: "/roles/:name",
    title: "角色表单",
    element: <RoleInfo />,
  },
  {
    path: "/roles/:id",
    title: "角色表单",
    element: <DemoPage />,
  },
  {
    path: "/options/system",
    title: "系统设置",
    element: <ConfigPage />,
  },
  {
    path: "/options/nav",
    title: "菜单设置",
    element: <MenuPage />,
  },
  {
    path: "/options/valueSpaces",
    title: "字典列表",
    element: <ValueSpacePage />,
  },
  {
    path: "/options/valueSpaces/:name",
    title: "字典表单",
    element: <ValueSpaceInfo />,
  },
];

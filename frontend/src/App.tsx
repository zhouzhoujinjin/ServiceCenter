import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AccountLayout, DefaultLayout } from "./layouts";
/**
 * 路由配置应用程序路由及其组件布局。
 * 它使用 React Router v6 为每个布局定义路由。
 * AccountLayout 和 DefaultLayout 组件决定共享 UI
 * 对于使用该布局的页面。
 */

const routeBase = import.meta.env.VITE_ROUTE_BASE;

function App() {
  return (
    <BrowserRouter basename={routeBase}>
      <Routes>
        <Route path="/account/*" element={<AccountLayout />} />
        <Route path="*" element={<DefaultLayout />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;

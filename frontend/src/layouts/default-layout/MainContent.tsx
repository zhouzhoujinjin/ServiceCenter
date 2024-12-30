import { Outlet, Route, Routes } from "react-router-dom";
import classes from "./MainContent.module.css";
import { routes } from "~/routes";
import KeepAlive from "../KeepAlive";

export const MainContent = () => {
  return (
    <div className={classes.mainContent}>
      <Routes>
        <Route element={<KeepAlive />}>
          {routes.map((x) => (
            <Route key={x.path} path={x.path} element={x.element} />
          ))}
        </Route>
      </Routes>
      <Outlet />
    </div>
  );
};

import { useSystemOptions } from "~/hooks/useSystemOptions";
import classes from "./Sidebar.module.css";
import clsx from "clsx";
import { FullMenu } from "./FullMenu";
import { useAuth, useTabLayout } from "~/hooks";
import { useEffect, useState } from "react";
import { MenuItem } from "~/types";

export const Sidebar = () => {
  const {
    uiOptions: { siderCollapsed },
  } = useSystemOptions();

  const { nav } = useAuth();
  const { addTab } = useTabLayout();

  const [menuItems, setMenuItems] = useState<MenuItem[]>([]);

  useEffect(() => {
    if (nav) {
      // console.log(nav)
      setMenuItems(nav);
    }
  }, [nav]);

  return (
    <div className={clsx(classes.root, siderCollapsed && classes.collapsed)}>
      <FullMenu
        items={menuItems}
        activeKey=""
        variant={siderCollapsed ? "collapsed-nav" : "nav"}
        onChange={(item) => addTab(item, 'end')}
      />
    </div>
  );
};

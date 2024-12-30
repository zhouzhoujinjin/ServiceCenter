import { FC } from "react";
import { PropsWithChildren } from "react";

import classes from "./SecondarySidebar.module.css";

export const SecondarySidebar: FC<PropsWithChildren<{
  w?: number|string
}>> = ({ children, w}) => {
  return <div className={classes.secondarySidebar} style={{width: w||260}}>{children}</div>;
};

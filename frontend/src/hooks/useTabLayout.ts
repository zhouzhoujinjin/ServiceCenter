import { useContext } from "react";
import { TabLayoutContext } from "./TabLayoutProvider";

export const useTabLayout = () => {
  const context = useContext(TabLayoutContext);
  if (context == null) {
    throw new Error("未提供 TabLayoutProvider");
  }

  return context;
};

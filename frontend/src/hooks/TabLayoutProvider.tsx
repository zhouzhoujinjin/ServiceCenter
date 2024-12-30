import { FC, PropsWithChildren, createContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { MenuItem } from "~/types";

type TabLayoutContextValue = {
  items: TabItem[];
  activePath: string;
  addTab: (item: TabItem, position: Position) => void;
  removeTab: (path: string) => void;
  updateTab: (path: string, item: TabItem) => void;
  setActive: (path: string) => void;
};

type TabItem = Omit<MenuItem, "children">;

type Position = "start" | "end" | "next-active" | "previous-active" | number;

export const TabLayoutContext = createContext<TabLayoutContextValue | null>(
  null
);

export const TabLayoutProvider: FC<PropsWithChildren> = ({ children }) => {
  const [state, setState] = useState<
    Pick<TabLayoutContextValue, "items" | "activePath">
  >({
    items: [],
    activePath: "",
  });

  const navigate = useNavigate();

  useEffect(() => {
    const tabsJson = window.localStorage.getItem("OPEN_TABS")
    if (tabsJson) {
      const tabs = JSON.parse(tabsJson)
      setState(tabs)
    }
  }, [])

  const saveTabs = (items: TabItem[], activePath: string) => {

    window.localStorage.setItem("OPEN_TABS", JSON.stringify({ items, activePath }, (k, v) => {
      if(k === 'children') {
        return null
      } else {
        return v
      }
    }))
  }

  const addTab = (item: TabItem, position: Position) => {
    if (state.items.map((x) => x.uri).includes(item.uri)) {
      setActive(item.uri);      
      return;
    }

    const newItems = [...state.items];
    let pos = -1;
    switch (position) {
      case "start":
        pos = 0;
        break;
      case "end":
        pos = newItems.length;
        break;
      case "next-active":
        {
          const index = newItems.findIndex((x) => x.uri === state.activePath);
          pos = index > -1 ? index + 1 : newItems.length;
        }
        break;
      case "previous-active":
        {
          const index = newItems.findIndex((x) => x.uri === state.activePath);
          pos = index > -1 ? index : newItems.length;
        }
        break;
    }
    newItems.splice(pos, 0, item);
    setState((s) => ({ ...s, items: newItems, activePath: item.uri }));
    saveTabs(newItems, item.uri)
    navigate(item.uri);
  };
  const removeTab = (path: string) => {
    const newItems = [...state.items];
    const index = newItems.findIndex((x) => x.uri === path);
    if (index > -1) {
      const nextActiveIndex = index - 1 >= 0 ? index - 1  : index + 1 < newItems.length ? index : -1;

      newItems.splice(index, 1);
      const ap = nextActiveIndex > -1 ? newItems[nextActiveIndex].uri : "/home";
      setState((s) => ({
        ...s,
        items: newItems,
        activePath: ap,
      }));
      saveTabs(newItems, ap)
      navigate(ap);
    }
  };
  const updateTab = (path: string, item: TabItem) => {
    const newItems = [...state.items];
    const pos = newItems.findIndex((x) => x.uri === path);
    if (pos > -1) {
      newItems[pos] = item;
      saveTabs(newItems, state.activePath)
      setState((s) => ({ ...s, items: newItems }));
    }
  };

  const setActive = (path: string) => {
    if (path !== state.activePath) {
      
      saveTabs(state.items, path)
      setState((s) => ({ ...s, activePath: path }));
      navigate(path);
    }
  };

  return (
    <TabLayoutContext.Provider
      value={{
        ...state,
        addTab,
        removeTab,
        updateTab,
        setActive,
      }}
    >
      {children}
    </TabLayoutContext.Provider>
  );
};

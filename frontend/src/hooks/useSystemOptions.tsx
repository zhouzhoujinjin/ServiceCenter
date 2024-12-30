import {
  FC,
  PropsWithChildren,
  createContext,
  useContext,
  useEffect,
  useState,
} from "react";
import { GetConfig } from "~/shared/system";

const SIDER_COLLAPSED_KEY = "SIDER_COLLAPSED_KEY";

export type SystemOptions = {
  uiOptions: {
    hasSider?: boolean;
    siderCollapsed?: boolean;
    hasHeader?: boolean;
    theme?: string;
  };
  platformOptions: Record<string, any>;
};

const defaultOptions = {
  uiOptions: {
    hasSider: import.meta.env.VITE_UI_HASSIDER || true,
    siderCollapsed: window.localStorage.getItem(SIDER_COLLAPSED_KEY) === "true",
    hasHeader: import.meta.env.VITE_UI_HASHEADER || true,
  },
  platformOptions: {},
};

type SystemOptionsContextOptions = SystemOptions & {
  refresh: () => void;
  updateSiderCollapsed: (collapsed: boolean) => void;
};

const SystemOptionsContext = createContext<SystemOptionsContextOptions | null>(
  null
);

export const SystemOptionsProvider: FC<PropsWithChildren> = (props) => {
  const [updated, setUpdated] = useState(false);
  const [state, setState] = useState<SystemOptionsContextOptions>({
    ...defaultOptions,
    refresh: () => setUpdated(!updated),
    updateSiderCollapsed: (collapsed: boolean) => {
      setState((s) => ({
        ...s,
        uiOptions: {
          ...s.uiOptions,
          siderCollapsed: collapsed,
        },
      }));
      window.localStorage.setItem(
        SIDER_COLLAPSED_KEY,
        JSON.stringify(collapsed)
      );
    },
  });

  useEffect(() => {
    GetConfig((config:any) => {
      setState((s) => ({
        ...s,
        platformOptions: config || {},
      }));
    });
  }, [updated]);

  return (
    <SystemOptionsContext.Provider value={state}>
      {props.children}
    </SystemOptionsContext.Provider>
  );
};

export const useSystemOptions = () => {
  const data = useContext(SystemOptionsContext);
  if (data === null) {
    throw new Error(
      "useSystemOptions must be used within a SystemOptionsProvider"
    );
  }
  return data;
};

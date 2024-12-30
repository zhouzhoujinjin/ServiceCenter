import { Loader } from "@mantine/core";
import React, { FC, PropsWithChildren, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  GetAccountMenu,
  GetAccountPermissions,
  GetAccountProfile,
} from "~/shared/account-requests";
import { token } from "~/shared/token";
import { AccountInfos } from "~/types";
import classes from "./AuthProvider.module.css";

export const ClearCached = () => {
  window.sessionStorage.removeItem("permissions");
  window.sessionStorage.removeItem("profile");
};

type AuthContextProps = AccountInfos & {
  refresh: () => void;
  status: "pending" | "success" | "error";
};

export const AuthContext = React.createContext<AuthContextProps | null>(null);

export const AuthProvider: FC<PropsWithChildren<{ loginRoute: string }>> = ({
  children,
  loginRoute,
}) => {
  const navigate = useNavigate();
  const [updated, setUpdated] = useState(false);
  const [state, setState] = useState<AuthContextProps>({
    user: { userName: "" },
    permissions: [],
    nav: [],
    status: "pending",
    refresh: () => {
      ClearCached();
      setUpdated(!updated);
    },
  });
  useEffect(() => {
    if (token.get()) {
      GetAccountMenu((data: any) => {
        setState((f) => ({
          ...f,
          nav: data,
        }));
      });
      GetAccountProfile(
        (profiles: any) => {
          // console.log('getProfile')
          setState((u) => ({
            ...u,
            status: "success",
            profiles,
          }))
          if (profiles.userName) {
            setState((u) => ({
              ...u,
              user: { userName: profiles.userName }
            }))
          }
        },
        () => {
          setState((f) => ({
            ...f,
            status: "error",
          }));
        }
      );
      GetAccountPermissions(
        (permissions: any) =>{
          console.log('getPermission')
          setState((u) => ({
            ...u,
            status: "success",
            permissions,
          }))},
        () => {
          setState((f) => ({
            ...f,
            status: "error",
          }));
        }
      );
    } else {
      navigate(loginRoute);
    }
  }, [loginRoute, updated]);
  if (state.status === "error") {
    navigate(loginRoute);
  }
  return (
    <AuthContext.Provider value={state}>
      {state.status !== "success" ? (
        <div className={classes.root}>
          <Loader />
        </div>
      ) : (
        children
      )}
    </AuthContext.Provider>
  );
};

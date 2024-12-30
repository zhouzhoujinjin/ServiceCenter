import { useContext } from "react";
import { AuthContext } from "./AuthProvider";

export const useAuth = () => {
  const state = useContext(AuthContext);  
  // console.log(state)
  if (state === null) {
    throw new Error("useAuth must be used within a AuthProvider");
  }

  const isPending = state.status === "pending";
  const isError = state.status === "error";
  const isSuccess = state.status === "success";
  const isAuthenticated = state && isSuccess;

  const updateProfile = () => {    
    state.refresh();
  };

  const checkProfile = (key: string, value?: string) => {
    if (value) {
      return state.profiles && state.profiles[key] === value;
    }
    return state.profiles && state.profiles[key];
  };
  const checkPermission = (perm: string | string[]) => {
    if (typeof perm === "string") {
      return (
        state.status &&
        state.permissions &&
        state.permissions.length > 0 &&
        state.permissions.indexOf(perm) > -1
      );
    }
    if (Array.isArray(perm)) {
      return (
        state.permissions &&
        state.permissions.length &&
        state.permissions.filter(Set.prototype.has, new Set(perm)).length
      );
    }
  };

  return {
    ...state,
    isPending,
    isError,
    isSuccess,
    isAuthenticated,
    updateProfile,
    checkPermission,
    checkProfile,
  };
};

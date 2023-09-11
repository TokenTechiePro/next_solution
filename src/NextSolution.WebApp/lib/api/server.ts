import { apiConfig } from "@/config/api";

import { getCookies } from "../cookies/server";
import { Api } from "./core";
import { ApiState, User } from "./types";

export const getApi = (): Api => {
  const cookies = getCookies();
  const api = new Api({ ...apiConfig, store: cookies });
  return api;
};

export const getUser = (params?: { onUnauthenticated?: () => void }): User | null | undefined => {
  const cookies = getCookies();
  const api = new Api({ ...apiConfig, store: cookies });
  const user = api.state.getValue<ApiState>().user;
  if (!user) params?.onUnauthenticated?.();
  return user;
};

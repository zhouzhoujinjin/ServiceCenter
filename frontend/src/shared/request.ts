import ky, { Options } from "ky";
import { notifications } from "@mantine/notifications";
import { token } from "./token";

export interface AjaxResp<T> {
  code: number;
  message?: string;
  data?: T;
}

export interface PagedAjaxResp<T> extends AjaxResp<T[]> {
  page?: number;
  total?: number;
  // totalPage?: number;
  // nextPageToken?: string | number;
  // prevPageToken?: string | number;
}

export type RequestCallback<T> = (data?: T) => void;

const API = import.meta.env.VITE_API_HOST;
/**
 * Makes a request using the ky client with default options.
 * Handles global error notification.
 */

const internalApi = ky.extend({
  timeout:240000,
  hooks: {
    beforeRequest: [
      (request) => {
        if (request.url.includes(API)) {
          const tokenStr = token.get();
          request.headers.set("Authorization", `Bearer ${tokenStr}`);
        }

        if (request.method === "POST" && !request.url.endsWith('avatar') && !request.url.endsWith('files')) {
          request.headers.set("Content-Type", "application/json");
        }

        return request;
      },
    ],
  },
});


export const api = {
  /**
   * Makes an async GET request to the given URL using the internalApi client.
   *
   * @param url - The URL to make the GET request to.
   * @param options - Optional request options to pass to internalApi.
   *
   * @returns The response data if successful, or throws an error.
   * The response is typed and will be deserialized from JSON.
   *
   * Handles global error notification on failure.
   */
  get: async <T>(url: string, options?: Options) => {
    try {
      const result: AjaxResp<T> = await internalApi.get(url, options).json();

      const { code, message, data, ...rest } = result;
      if (message) {
        notifications.show({
          color: !code ? "green" : "red",
          message: result.message,
          autoClose: !code ? 2000 : 4000,
          maw: "10rem",
          w: "auto",
          radius: "lg",
          withCloseButton: false,
        });
      }
      if (!code) {
        return Object.keys(rest).length === 0 ? data : { ...rest, data };
      }
    } catch (e: any) {
      console.log(e);
      notifications.show({
        color: "red",
        title: "发生错误",
        message: e.toString(),
        radius: "lg",
        autoClose: false,
        withCloseButton: true,
      });
    }
  },
  /**
   * Makes an async POST request to the given URL using the internalApi client.
   *
   * @param url - The URL to make the POST request to.
   * @param postData - The data to POST in the request body.
   * @param options - Optional request options to pass to internalApi.
   *
   * @returns The response data if successful, or throws an error.
   * The response is typed and will be deserialized from JSON.
   *
   * Handles global error notification on failure.
   */
  post: async <T>(url: string, postData: unknown, options?: Options) => {
    try {
      const result: AjaxResp<T> = await internalApi
        .post(url, {
          ...options,
          json: postData,
        })
        .json();

      const { code, message, data, ...rest } = result;
      if (message) {
        notifications.show({
          color: !code ? "green" : "red",
          message: result.message,
          autoClose: !code ? 2000 : 4000,
          maw: "10rem",
          w: "auto",
          radius: "lg",
          withCloseButton: false,
        });
      }
      if (!code) {
        return Object.keys(rest).length === 0 ? data : { ...rest, data };
      }
    } catch (e: any) {
      console.log(e);
      notifications.show({
        color: "red",
        title: "发生错误",
        message: e.toString(),
        radius: "lg",
        autoClose: false,
        withCloseButton: true,
      });
    }
  },
  /**
   * Update file through a POST request to the given URL with FormData containing the given files.
   *
   * @param url - The URL to make the POST request to
   * @param files - The files to include in the FormData
   * @param options - Request options
   * @returns The response data
   */
  file: async <T>(
    url: string,
    files: Record<string, File>,
    options?: Options
  ) => {
    try {
      const formData = new FormData();
      Object.keys(files).forEach((x) => formData.append(x, files[x]));

      const result: AjaxResp<T> = await internalApi
        .post(url, {
          ...options,

          body: formData,
        })
        .json();

      const { code, message, data, ...rest } = result;
      if (message) {
        notifications.show({
          color: !code ? "green" : "red",
          message: result.message,
          autoClose: !code ? 2000 : 4000,
          maw: "10rem",
          w: "auto",
          radius: "lg",
          withCloseButton: false,
        });
      }
      if (!code) {
        return Object.keys(rest).length === 0 ? data : { ...rest, data };
      }
    } catch (e: any) {
      console.log(e);
      notifications.show({
        color: "red",
        title: "发生错误",
        message: e.toString(),
        radius: "lg",
        autoClose: false,
        withCloseButton: true,
      });
    }
  },
};

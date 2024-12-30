import { WithChildren } from "~/types";

export const getFriendlyUserName = (user:any, showFullName = true) =>
  user.profiles.fullName
    ? `${user.profiles.fullName} (${user.userName})`
    : user.userName;

export const loop = <T>(
  array: WithChildren<T>[],
  callback: (
    item: WithChildren<T>,
    index: number,
    array: WithChildren<T>[],
    parent?: WithChildren<T>
  ) => void,
  parent?: WithChildren<T>
) => {
  array.forEach((item, index, arr) => {
    callback(item, index, arr, parent);

    if (item.children) {
      loop(item.children, callback, item);
    }
  });
};

export const requestFullScreen = (element: HTMLElement) => {
  const requestMethod = element.requestFullscreen;
  if (requestMethod) {
    requestMethod.call(element);
  }
};

export const exitFullScreen = () => {
  const exitMethod = document.exitFullscreen;
  if (exitMethod) {
    exitMethod.call(document);
  }
};

export const parseToPossibleValue = (value: any) => {
  if (value === null || value === undefined) return "";
  if (!isNaN(+value)) return +value;
  if (value === "false") return false;
  if (value === "true") return true;
  if (value[0] === "[" || value[0] === "{") {
    try {
      return JSON.parse(value);
    } catch {
      // It's just a string, do nothing
    }
  }
  return value;
};

export const uuid = (len = 8, radix = 16) => {
  const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".split(
    ""
  );
  const value: string[] = [];
  let i = 0;
  radix = radix || chars.length;
  if (len) {
    // Compact form
    for (i = 0; i < len; i++) value[i] = chars[0 | (Math.random() * radix)];
  } else {
    // rfc4122, version 4 form
    let r;
    // rfc4122 requires these characters
    /* eslint-disable-next-line */
    value[8] = value[13] = value[18] = value[23] = "-";
    value[14] = "4";
    // Fill in random data.  At i==19 set the high bits of clock sequence as
    // per rfc4122, sec. 4.1.5
    for (i = 0; i < 36; i++) {
      if (!value[i]) {
        r = 0 | (Math.random() * 16);
        value[i] = chars[i === 19 ? (r & 0x3) | 0x8 : r];
      }
    }
  }
  return value.join("");
};



export const aggregateData = (data: any[],aggregatePropName:string) => {
  const noGroup = '未分组'
  if (data.length) {
    const grouped = data.reduce((acc, curr) => {
      const key = curr[aggregatePropName] || noGroup;
      if (!acc[key]) {
        acc[key] = [];
      }
      acc[key].push(curr);
      return acc;
    }, {});
    return grouped as Record<string,any[]>
  } return null
}
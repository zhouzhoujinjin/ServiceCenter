import { FC, useEffect, useState } from "react";
import { GetUserList } from "./service/pickUsers";
import {
  ComboboxItem,
  MultiSelect,
  Select,
  OptionsFilter,
  rem,
} from "@mantine/core";
import { IconUsers, IconUser } from "@tabler/icons-react";
import { ValueLabelInfo } from "~/types";

export const PickUsers: FC<{
  selectType: "single" | "multi";
  selectedUsers?: number[] | undefined;
  onChange?: (data: any) => void;
}> = ({ selectType = "single", selectedUsers, onChange }) => {
  const [usersInfo, setUsersInfo] = useState<any[]>([]);
  const [users, setUsers] = useState<any[]>([]);

  const optionsFilter: OptionsFilter = ({ options, search }) => {
    const splittedSearch = search.toLowerCase().trim().split(" ");
    return (options as ComboboxItem[]).filter((option) => {
      const words = option.label.toLowerCase().trim().split(" ");
      return splittedSearch.every((searchWord) =>
        words.some((word) => word.includes(searchWord))
      );
    });
  };

  useEffect(() => {
    GetUserList((data) => {
      if (data) {        
        setUsersInfo(data);
        const list: ValueLabelInfo[] = data.map((d) => ({
          value: d.id?.toString(),
          label: d.profiles?.fullName || d.userName,
        }));
        setUsers(list);
      }
    });
  }, []);

  return selectType === "single" ? (
    <Select
      w="100%"
      mb="sm"
      label="选取用户"
      placeholder="请选择用户"
      filter={optionsFilter}
      searchable
      onChange={(value) => {
        const selectedUser = usersInfo.find((u) => value === u.id);
        onChange && onChange(selectedUser);
      }}
      rightSectionPointerEvents="none"
      rightSection={<IconUser style={{ width: rem(16), height: rem(16) }} />}
      data={users}
      value={selectedUsers && String(selectedUsers[0])}
    ></Select>
  ) : (
    <MultiSelect
      w="100%"
      mb="sm"
      label="选取用户"
      placeholder="请选择用户"
      filter={optionsFilter}
      searchable
      onChange={(values) => {
        if (values) {          
          const selectedUsers = usersInfo.filter(
            (u) => values.indexOf(u.id.toString()) > -1
          );          
          onChange && onChange(selectedUsers);
        }
      }}
      rightSectionPointerEvents="none"
      rightSection={<IconUsers style={{ width: rem(16), height: rem(16) }} />}
      data={users}
      value={selectedUsers && selectedUsers.map(su => String(su))}
    ></MultiSelect>
  );
};

import { Button, Group, Select, SimpleGrid, Switch, TextInput } from "@mantine/core";
import { useForm } from "@mantine/form";
import { FC, useEffect, useState } from "react";
import { MenuItem } from "~/types";
import { CreateMenu, UpdateMenu } from "./services/menu";


export const MenuInfo: FC<{ values?: MenuItem, submitAction?: (values: any) => void }> = (
  {values, submitAction}
) => {

  const { onSubmit, getInputProps, setValues, reset } = useForm<MenuItem>({
    initialValues: {
      title: "",
      iconType: undefined,
      icon: "",
      uri: "",
      type: "route",
      invisible: false,
      childrenInvisible: true
    }
  });
  const iconTypes = [{ label: "内置", value: "builtin" }, { label: "图片", value: "image" }]
  const menuTypes = [{ label: "路由", value: "route" }, { label: "菜单组", value: "group" }]
  const [submitType, setSubmitType] = useState<string>("create");


  useEffect(() => {
    if (values) {
      setValues(values);
      setSubmitType("update");
    }
  }, [values, setValues])


  return (
    <form onSubmit={onSubmit((values) => {
      if (submitType === 'create') {
        CreateMenu(values as MenuItem, () => {
          submitAction && submitAction(values);
          reset();
        })
      } else {        
        UpdateMenu(values as MenuItem, () => {
          submitAction && submitAction(values);
          reset();
        })
      }
    })}
    >
      <TextInput
        withAsterisk
        label="菜单名称"
        placeholder="请输入名称"
        {...getInputProps("title")}
      />
      <Select
        label="图标类型"
        data={iconTypes}
        {...getInputProps("iconType")}
      />
      <TextInput
        label="图标样式"
        placeholder="请输入名称"
        {...getInputProps("icon")}
      />
      <Select
        withAsterisk
        label="菜单类型"
        data={menuTypes}
        {...getInputProps("type")}
      />
      <TextInput
        withAsterisk
        label="地址"
        placeholder="请输入地址"
        {...getInputProps("uri")}
      />

      <SimpleGrid cols={2} mb={10} mt={30} w={420}>
        <Switch
          label="不显示"
          {...getInputProps("invisible", { type: "checkbox" })}
        />
        <Switch
          label="不显示子菜单"
          {...getInputProps("childrenInvisible", { type: "checkbox" })}
        />
      </SimpleGrid>

      <Group justify="flex-end" mt="md">
        <Button type="submit">{submitType === 'update' ? "保存" : "新增"}</Button>
      </Group>
    </form>
  )

}
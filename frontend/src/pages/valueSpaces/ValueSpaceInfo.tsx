import { useEffect } from "react";
import { ValueSpacesProps } from "./types";
import { useForm } from "@mantine/form";
import { useParams } from "react-router-dom";
import { GetValueSpaceInfo, UpdateValueSpace } from "./services/valueSpaces";
import { ActionIcon, Box, Button, Divider, Group, TextInput } from "@mantine/core";
import { IconTrash } from "@tabler/icons-react";
import { uuid } from "~/shared";

export const ValueSpaceInfo = () => {

  const { name: valueSpaceName } = useParams();
  // const { setValues } = useForm();
  // const [fields, setFields] = useState()

  const form = useForm<ValueSpacesProps>({
    initialValues: {
      name: "",
      title: "",
      conditions: {},
      conditionArray: [],
      valueSpaceType: "Code",
      configureLevel: "system"
    }
  })

  useEffect(() => {
    if (valueSpaceName) {
      GetValueSpaceInfo(valueSpaceName, (data) => {
        if (data && data.conditions) {
          if (data.valueSpaceType === "Code") {
            const keys = Object.keys(data.conditions)
            const array = keys.map(k => ({
              code: k,
              value: data.conditions[k]
            }))
            data.conditionArray = array as []
          }
          form.setValues(data)
        }
      })
    }
  }, [])

  const onFormSubmit = (values: ValueSpacesProps) => {
    const vs = values;
    if (values.valueSpaceType === "Code") {
      if (values.conditionArray && values.conditionArray.length > 0) {
        vs.conditions = values.conditionArray.reduce((map: Record<string, any>, obj) => {
          map[obj.code] = obj.value;
          return map;
        }, {});
      }
    }
    UpdateValueSpace(vs, () => {

    })
  }

  const renderConditions = (vsType: string) => {
    switch (vsType) {
      case "Code":
        if (form.values.conditionArray!.length) {
          return (
            <div>{
              form.values.conditionArray!.map((item: any, index) => (
                <Group mt="xs" key={item.code}>
                  <TextInput
                    withAsterisk
                    {...form.getInputProps(`conditionArray.${index}.code`)}
                  />
                  <TextInput
                    withAsterisk
                    {...form.getInputProps(`conditionArray.${index}.value`)}
                  />
                  <ActionIcon color="red" onClick={() => form.removeListItem('conditionArray', index)}>
                    <IconTrash size="1rem" />
                  </ActionIcon>
                </Group>
              ))}
              <Group justify="center" mt="md">
                <Button
                  onClick={() =>
                    form.insertListItem('conditionArray', { key: uuid(), value: '' })
                  }
                >
                  添加新值
                </Button>
              </Group>
            </div>
          );
        }
        return null;

      case "Regex":
        return (
          <Group mt="xs" grow wrap="nowrap">
            <TextInput
              withAsterisk
              label="正则表达式"
              {...form.getInputProps(`conditions`)}
            />
          </Group>
        );
    }
  };


  return (
    <Box maw={600} mx="auto">
      <form onSubmit={form.onSubmit((values) => {
        onFormSubmit(values);
      })}>
        <TextInput
          withAsterisk
          label="值空间名称"
          mb='md'
          disabled
          {...form.getInputProps("name")}
        />
        <TextInput
          withAsterisk
          label="值空间标题"
          mb='md'
          {...form.getInputProps("title")}
        />
        <TextInput
          withAsterisk
          label="系统级别"
          disabled
          mb='md'
          {...form.getInputProps("configureLevel")}
        />
        <TextInput
          withAsterisk
          label="值空间类型"
          disabled
          mb='md'
          {...form.getInputProps("valueSpaceType")}
        />
        <Divider />
        {
          renderConditions(form.values.valueSpaceType)
        }
        <Group justify="flex-end" mt="md">
          <Button type="submit">保存</Button>
        </Group>
      </form>
    </Box>)
}
import { FC, useEffect } from "react";
import { GetDepartmentUsers, UpdateDepartment } from "./services/department";
import { Department, DepartmentUser } from "./types";
import { useForm } from "@mantine/form";
import { Button, Card, Group, NumberInput, Switch, Text, TextInput, Title } from "@mantine/core";
import { PickUsers } from "~/components/picker-users";

export const DepartmentInfo: FC<{ departmentId: number, submitAction?: () => void }> = ({
  departmentId,
  submitAction
}) => {
  const { onSubmit, getInputProps, values, setValues, reset } = useForm<Department>({
    initialValues: { id: 0, title: "", users: [] },
  });
  const initUser = {
    position: '职员',
    isUserMajorDepartment: false,
    level: 0,
  }

  useEffect(() => {
    if (departmentId) {
      GetDepartmentUsers(departmentId, (data) => {
        reset();
        if (data) {
          setValues(data);
        }
      });
    }
  }, [departmentId, reset, setValues]);

  const save = (department: Department) => {
    if (department) {
      UpdateDepartment(department.id, department, submitAction);
    }
  }

  const fields = () => {
    if (values.users && values.users.length) {
      return values.users.map((item, index) => (
        <Group key={item.userId} mb="sm">
          <Title size="h6">姓名</Title>
          <Text fz='sm' w='4em'>{item.profiles?.fullName}</Text>
          <Title size="h6">职务</Title>
          <TextInput
            placeholder="职务"
            withAsterisk
            style={{ flex: 1, width: 60 }}
            {...getInputProps(`users.${index}.position`)}
          />
          <Title size="h6">级别</Title>
          <NumberInput
            placeholder="级别"
            withAsterisk
            style={{ flex: 1, width: 10 }}
            {...getInputProps(`users.${index}.level`)}
          />
          <Title size="h6">是否主部门</Title>
          <Switch
            label="主部门"
            {...getInputProps(`users.${index}.isUserMajorDepartment`, {
              type: "checkbox",
            })}
          />
        </Group>
      ))
    }
    return null;
  }



  return (
    <Card w="90%">
      <form onSubmit={onSubmit((values) => {
        save(values)
      })}>
        <TextInput
          mb="sm"
          {...getInputProps("title")}
          label="部门名称"
          placeholder=""
        />
        <PickUsers
          selectType="multi"
          onChange={(data) => {
            if (data) {
              const newUsers: DepartmentUser[] = []
              data.forEach((d: DepartmentUser) => {
                const index = values.users!.findIndex(x => x.userId === d.id);
                if (index > -1) {
                  newUsers.push(values.users![index]);
                } else {
                  newUsers.push({
                    ...d,
                    userId: d.id,
                    ...initUser
                  })
                }
              });
              setValues({ ...values, users: newUsers });
            }
          }}
          selectedUsers={values.users!.map((x) => Number(x.userId))}
        />
        {fields()}
        <Button type="submit">保存</Button>
      </form>
    </Card>
  );
};

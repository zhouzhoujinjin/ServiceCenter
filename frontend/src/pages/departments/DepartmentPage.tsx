import { useCallback, useEffect, useState } from "react";
import {
  Button,
  // Card,
  Group,
  Modal,
  // Paper,
  Stack,
  TextInput,
  Tooltip,
} from "@mantine/core";
import { Tree, TreeItem } from "~/components/dnd-tree";
import { Department } from "./types";
import {
  CreateDepartment,
  GetDepartmentTree,
  RemoveDepartment,
  UpdateDepartmentStruct,
} from "./services/department";
import { ActionIcon } from "@mantine/core";
import { IconTrash } from "@tabler/icons-react";

import { useDisclosure } from "@mantine/hooks";
import { useForm } from "@mantine/form";
import { DepartmentInfo } from "./DepartmentInfo";
import { SecondarySidebar } from "~/layouts/default-layout/SecondarySidebar";
import { FlattenedItem } from "~/components/dnd-tree/types";

export const DepartmentPage = () => {
  const [departments, setDepartments] = useState<Department[]>([]);
  const [selectedDepartmentId, setSelectedDepartmentId] = useState(0);
  const [opened, { close, open }] = useDisclosure(false);
  const form = useForm({ initialValues: { title: "" } });

  const handleRemove = (id: number) => {
    RemoveDepartment(id, () => {
      refreshTree();
    });
  };

  const create = (formValues: any) => {
    CreateDepartment(formValues as Department, () => {
      refreshTree();
    });
  };

  const updateStruct = (departmentId: number, parentId: number) => {
    UpdateDepartmentStruct(departmentId, parentId);
  }

  const refreshTree = useCallback(() => {
    GetDepartmentTree((data) => {
      if (data) {
        setDepartments(data);
      }
    });
  }, []);

  useEffect(() => {
    refreshTree();
  }, [refreshTree]);

  return (
    <div style={{ display: "flex" }}>
      <SecondarySidebar w={750}>
        <Group justify="space-between" mb="sm">
          <h4 style={{ margin: 0 }}>部门树</h4>
          <Button
            size="sm"
            onClick={() => {
              open();
            }}
          >
            创建部门
          </Button>
        </Group>
        <Tree
          items={departments as TreeItem[]}
          onChange={(item, items) => {
            if (item.id) {
              const node = item as FlattenedItem;
              updateStruct(Number(node.id), Number(node.parentId));
            }
            setDepartments(items as Department[])
          }}
          onContentClick={(item) => {
            setSelectedDepartmentId(item && Number(item.id))
          }}
          collapsible
          indicator
          indentationWidth={24}
          itemContentRender={(item: any) => item.title.length > 6 ? <Tooltip label={item.title}><span>{item.title}</span></Tooltip> : item.title}
          itemRightSection={(item: any) => (
            <ActionIcon variant="subtle" onClick={() => handleRemove(item.id)}>
              <IconTrash width={16} height={16} />
            </ActionIcon>
          )}
        />
      </SecondarySidebar>
      <Stack w='100%'>
        <DepartmentInfo departmentId={selectedDepartmentId} submitAction={() => { refreshTree() }} />
      </Stack>
      <Modal opened={opened} onClose={close} title="创建部门" centered>
        <form
          onSubmit={form.onSubmit((values) => {
            create(values);
            form.reset();
            close();
          })}
        >
          <TextInput
            withAsterisk
            label="部门名称"
            placeholder="请输入名称"
            {...form.getInputProps("title")}
          />
          <Group justify="flex-end" mt="md">
            <Button type="submit">保存</Button>
          </Group>
        </form>
      </Modal>
    </div>
  );
};

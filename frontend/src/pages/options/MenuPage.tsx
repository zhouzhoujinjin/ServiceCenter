import { useCallback, useEffect, useState } from "react";
import { Tree, TreeItem } from "~/components/dnd-tree";
import { GetAdminMenu, RemoveMenu, UpdateMenuStruct } from "./services/menu";
import { MenuItem } from "~/types";
import { ActionIcon, Button, Group, Modal, Stack } from "@mantine/core";
import { IconTrash } from "@tabler/icons-react";
import { SecondarySidebar } from "~/layouts/default-layout/SecondarySidebar";
import { useDisclosure } from "@mantine/hooks";
import { MenuInfo } from "./MenuInfo";
import { FlattenedItem } from "~/components/dnd-tree/types";



export const MenuPage = () => {
  const [menuItems, setMenuItems] = useState<TreeItem[]>([]);
  const [opened, { close, open }] = useDisclosure(false);
  const [selectedMenuItemValues, setSelectedMenuItem] = useState<MenuItem>();

  const handleRemove = (id: number) => {
    RemoveMenu(id,()=>{
      refreshTree();
    })
  };

  const updateStruct = (menuId: number, parentId: number) => {
    UpdateMenuStruct(menuId, parentId);
  }

  const refreshTree = useCallback(() => {
    GetAdminMenu((data) => {
      if (data) {
        setMenuItems(data as unknown as TreeItem[]);
      }
    });
  }, [])

  useEffect(() => {
    refreshTree();
  }, [refreshTree]);

  return (
    <div style={{ display: "flex" }}>
      <SecondarySidebar w={750}>
        <Group justify="space-between" mb="sm">
          <h4 style={{ margin: 0 }}>菜单树</h4>
          <Button
            size="sm"
            onClick={() => {
              open();
            }}
          >
            新菜单
          </Button>
        </Group>
        <Tree
          items={menuItems}
          onChange={(item, items) => {
            if (item.id) {
              const node = item as FlattenedItem;              
              updateStruct(Number(node.id), Number(node.parentId));
            }
            setMenuItems(items as TreeItem[])
          }}
          collapsible
          indicator
          indentationWidth={24}
          itemContentRender={(item: any) => item.title}
          onContentClick={(item) => {
            const menuItem = item as unknown as MenuItem;
            setSelectedMenuItem(menuItem);
          }}
          itemRightSection={(item: any) => (
            <ActionIcon variant="subtle" onClick={() => handleRemove(item.id)}>
              <IconTrash width={16} height={16} />
            </ActionIcon>
          )}
        />
      </SecondarySidebar>
      <Stack w='100%'>
        <MenuInfo values={selectedMenuItemValues} submitAction={(data) => {
          refreshTree();
          setSelectedMenuItem(data);          
        }}
        />
      </Stack>
      <Modal opened={opened} onClose={close} title="创建菜单" centered>
        <MenuInfo values={undefined} submitAction={() => { close(); refreshTree(); }} />
      </Modal>
    </div >
  );
};

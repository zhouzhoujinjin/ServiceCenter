import { useState } from "react"
import { Role } from "./types"
import { useCallback } from "react"
import { GetRoles, RemoveRole } from "./services/role"
import { useEffect } from "react"
import { useMemo } from "react"
import { MRT_ColumnDef, MantineReactTable, useMantineReactTable } from 'mantine-react-table'
import { ActionIcon, Badge, Box, Button, Group, Popover, Stack, useMantineTheme, } from '@mantine/core'
import { PagedAjaxResp } from "~/shared"
import { IconEdit, IconUser, IconX } from "@tabler/icons-react"
import { useTabLayout } from "~/hooks"
import { MenuItem } from "~/types"
import { MRT_Localization_ZH_HANS } from "~/shared/mantine-react-table-zh-cn";

export const RolePage = () => {
  const [roleList, setRoleList] = useState<Role[]>([])
  const [rowCount, setRowCount] = useState<number>(0);
  const [popOpened, setPopOpened] = useState('');
  // const [selectedRole, setSelectedRole] = useState<Role>();
  const { addTab } = useTabLayout();
  const { colors } = useMantineTheme();

  const refreshTable = useCallback(() => {
    GetRoles(data => {
      const { total, ...rest } = data as PagedAjaxResp<Role>;
      setRowCount(total || 0);
      setRoleList(rest.data || []);
    })
  }, [])

  useEffect(() => {
    refreshTable();
  }, [refreshTable])

  const columns = useMemo<MRT_ColumnDef<Role>[]>( //TS helps with the autocomplete while writing columns
    () => [
      {
        accessorKey: 'title', //normal recommended usage of an accessorKey
        header: '角色名称',
        size: 120,
        grow: false,
      },
      {
        accessorKey: 'users', //normal recommended usage of an accessorKey
        header: '角色成员',
        size: 240,
        grow: false,
        Cell: ({ row }: any) => {
          return (<div >
            <Group>
              {
                row.original.users.length ? row.original.users.map((u: any) => (
                  <Badge
                    variant="default"
                    radius={3}
                    key={u.id}
                  >{u.profiles && u.profiles.fullName || u.userName}
                  </Badge>
                )) : "尚未分配用户"
              }
            </Group>
          </div>)
        },
      },
    ],
    [],
  );

  const table = useMantineReactTable({
    columns,
    data: roleList,
    paginationDisplayMode: 'pages',
    manualPagination: true,
    rowCount: rowCount,
    localization: MRT_Localization_ZH_HANS,
    enableTopToolbar: false,
    // onPaginationChange: setPagination,
    mantinePaginationProps: {
      showRowsPerPage: false,
    },
    // state: { pagination },
    enableColumnFilters: false,
    enableGlobalFilter: false,
    // enableColumnResizing: true,
    // columnResizeMode: 'onChange',
    enableRowActions: true,
    positionActionsColumn: 'last',
    displayColumnDefOptions: {
      'mrt-row-actions': {
        // header: '操作', //change header text
        size: 120, //make actions column wider
      },
    },
    renderRowActions: ({ row }) => (

      <Box style={{ display: 'flex', flexWrap: 'nowrap', gap: '9px' }} >
        <ActionIcon
          variant="default"
          onClick={() => {
            // setSelectedRole(row.original);
            const roleInfo: MenuItem = {
              uri: `/roles/${row.original.name}`,
              title: '编辑角色'
            }
            addTab(roleInfo, 'end')
          }}>
          <IconEdit />
        </ActionIcon>

        <ActionIcon
          variant="default"
          onClick={() => {
            // setSelectedRole(row.original);
            const roleUsers: MenuItem = {
              uri: `/roles/${row.original.name}/users`,
              title: '角色用户'
            }
            addTab(roleUsers, 'end')
          }}>
          <IconUser />
        </ActionIcon>
        <Popover width={300} position="bottom" withArrow shadow="md" opened={popOpened === row.original.name}>
          <Popover.Target>
            <ActionIcon variant="default" onClick={() => { setPopOpened(row.original.name!) }} >
              <IconX />
            </ActionIcon>
          </Popover.Target>
          <Popover.Dropdown >
            <Group justify="center">
              <Button onClick={() => {
                if (row.original.name) {
                  RemoveRole(row.original.name, () => {
                    refreshTable()
                  })
                }
                setPopOpened('')
              }}>确定删除？</Button>
              <Button color="yellow" onClick={() => {
                setPopOpened('')
              }
              }>取消</Button>
            </Group>
          </Popover.Dropdown>
        </Popover>
      </Box>

    ),
    mantineTableProps: {
      withColumnBorders: true,
      highlightOnHoverColor: colors.blue[5],
    },
    mantineTableBodyCellProps: {
      // @ts-ignore
      sx: {
        flex: "0 0 auto",
      },
    },
  });


  return (<>
    <Stack>
      <Group justify="flex-end" wrap="nowrap">
        <Button onClick={() => {
          const roleInfo: MenuItem = {
            uri: `/roles/create`,
            title: '新建角色'
          }
          addTab(roleInfo, 'end')
        }}>创建角色</Button>
      </Group>
      <MantineReactTable table={table} />
    </Stack>
  </>)
}
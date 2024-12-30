import { useCallback, useEffect, useMemo, useState } from "react";
import { GetAdminUsers, RemoveUser } from "./services/user";
import { UserData } from "./types";
import {
  MRT_ColumnDef,
  MRT_PaginationState,
  useMantineReactTable,
} from "mantine-react-table";
import { MantineReactTable } from "mantine-react-table";
import "mantine-react-table/styles.css";
import { PagedAjaxResp } from "~/shared";
import {
  ActionIcon,
  Badge,
  Box,
  Button,
  Group,
  Input,
  Modal,
  Popover,
  Stack,
  useMantineTheme,
} from "@mantine/core";
import { IconEdit, IconSearch, IconX } from "@tabler/icons-react";
import { useDisclosure } from "@mantine/hooks";
import { UserInfo } from "..";
import dayjs from "dayjs";
import { MRT_Localization_ZH_HANS } from "~/shared/mantine-react-table-zh-cn";

export const UserPage = () => {
  const [userList, setUserList] = useState<UserData[]>([]);
  const [filters, setFilters] = useState<Record<string, string>>({});
  const [rowCount, setRowCount] = useState<number>(0);
  const [pagination, setPagination] = useState<MRT_PaginationState>({
    pageIndex: 0,
    pageSize: 10,
  });
  const [selectedUser, setSelectedUser] = useState<UserData>();
  const [popOpened, setPopOpened] = useState("");
  const [opened, { close, open }] = useDisclosure(false);
  const [searchValue, setSearchValue] = useState<string>("");
  const { colors } = useMantineTheme();

  const columns = useMemo<MRT_ColumnDef<UserData>[]>( //TS helps with the autocomplete while writing columns
    () => [
      {
        accessorKey: "userName", //normal recommended usage of an accessorKey
        header: "用户名",
        size: 120,
        grow: false,
      },
      {
        accessorKey: "birthday", //normal recommended usage of an accessorKey
        header: "出生日期",
        size: 120,
        grow: false,
        Cell: ({ row }) => {
          return (
            <div>
              {row.original.profiles?.birthday
                ? dayjs(row.original.profiles.birthday).format("YYYY-MM-DD")
                : ""}
            </div>
          );
        },
      },
      {
        accessorKey: "roles", //normal recommended usage of an accessorKey
        header: "所属角色",
        Cell: ({ row }) => {
          if (row.original && row.original.roles) {
            const keys = Object.keys(row.original.roles as object);
            return (<div>
              <Group>
                {
                  keys.map((k: string) => {
                    return (
                      <Badge key={k}
                        variant="default"
                        radius={3}
                      >
                        {row.original.roles && row.original.roles[k]}
                      </Badge>
                    );
                  })
                }
              </Group>
            </div>)
          }
          return null;
        },
        size: 240,
        grow: false,
      },
      {
        accessorKey: "profiles.fullName", //normal recommended usage of an accessorKey
        header: "真实姓名",
        Cell: ({ row }) => {
          return (
            <div>
              {(row.original.profiles && row.original.profiles.fullName) ||
                row.original.userName}
            </div>
          );
        },
        size: 120,
        grow: false,
      },
    ],
    []
  );

  const table = useMantineReactTable({
    columns,
    data: userList,
    paginationDisplayMode: "pages",
    manualPagination: true,
    enableTopToolbar: false,
    rowCount: rowCount,
    localization: MRT_Localization_ZH_HANS,
    onPaginationChange: setPagination,
    mantinePaginationProps: {
      showRowsPerPage: false,
    },
    state: { pagination },
    enableColumnFilters: false,
    enableGlobalFilter: false,
    // enableColumnResizing: true,
    // columnResizeMode: "onChange",
    enableRowActions: true,
    positionActionsColumn: "last",
    displayColumnDefOptions: {
      "mrt-row-actions": {
        // header: "操作", //change header text
        size: 120, //make actions column wider
      },
    },
    renderRowActions: ({ row }) => (
      <Box style={{ display: "flex", flexWrap: "nowrap", gap: "9px" }}>
        <ActionIcon
          variant="default"
          onClick={() => {
            setSelectedUser(row.original);
            open();
          }}
        >
          <IconEdit />
        </ActionIcon>
        <Popover
          width={300}
          position="bottom"
          withArrow
          shadow="md"
          opened={popOpened === row.original.userName}
        >
          <Popover.Target>
            <ActionIcon
              variant="default"
              onClick={() => {
                setPopOpened(row.original.userName);
              }}
            >
              <IconX />
            </ActionIcon>
          </Popover.Target>
          <Popover.Dropdown>
            <Group justify="center">
              <Button
                onClick={() => {
                  RemoveUser(row.original.userName, () => {
                    setPopOpened("");
                    refreshTable(
                      pagination.pageIndex,
                      pagination.pageSize,
                      filters
                    );
                  });
                }}
              >
                确定删除？
              </Button>
              <Button color="yellow" onClick={() => setPopOpened("")}>
                取消
              </Button>
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

  const refreshTable = useCallback(
    (pageIndex: number, pageSize: number, filters: Record<string, string>) => {
      GetAdminUsers(
        { page: pageIndex, pageSize: pageSize, filters },
        (data) => {
          const { total, ...rest } = data as PagedAjaxResp<UserData>;
          setRowCount(total || 0);
          setUserList(rest.data || []);
        }
      );
    },
    []
  );

  useEffect(() => {
    refreshTable(pagination.pageIndex, pagination.pageSize, filters);
  }, [pagination.pageIndex, pagination.pageSize, filters, refreshTable]);

  return (
    <>
      <Stack>
        <Group justify="flex-end" wrap="nowrap">
          <Input
            placeholder="关键字"
            onChange={(event) => {
              setSearchValue(event.target.value);
            }}
            rightSectionPointerEvents="all"
            // mt="md"
            rightSection={
              <ActionIcon
                variant="filled"
                aria-label="Settings"
                onClick={() => {
                  setFilters({ ...filters, query: searchValue });
                }}
              >
                <IconSearch stroke={1.5} />
              </ActionIcon>
            }
          />
          <Button
            onClick={() => {
              open();
            }}
          >
            添加用户
          </Button>
        </Group>
        <MantineReactTable table={table} />
      </Stack>
      <Modal
        opened={opened}
        onClose={() => {
          close();
          setSelectedUser(undefined);
        }}
        title="编辑用户"
        centered
      >
        <UserInfo
          userData={selectedUser}
          onSave={() => {
            close();
            setSelectedUser(undefined);
            refreshTable(pagination.pageIndex, pagination.pageSize, filters);
          }}
        />
      </Modal>
    </>
  );
};

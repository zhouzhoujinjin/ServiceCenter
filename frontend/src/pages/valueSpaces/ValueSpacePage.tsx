import { useCallback, useEffect, useMemo, useState } from "react";
import { ValueSpacesProps } from "./types";
import { GetValueSpaces } from "./services/valueSpaces";
import {
  MRT_ColumnDef,
  MantineReactTable,
  useMantineReactTable,
} from "mantine-react-table";
import { ActionIcon, Box, Stack, Title, useMantineTheme } from "@mantine/core";
import { IconEdit } from "@tabler/icons-react";
import { useTabLayout } from "~/hooks";
import { MenuItem } from "~/types";
import { MRT_Localization_ZH_HANS } from "~/shared/mantine-react-table-zh-cn";

export const ValueSpacePage = () => {
  const [list, setList] = useState<ValueSpacesProps[]>([]);
  const { addTab } = useTabLayout();
  const {colors} = useMantineTheme();

  const refreshTable = useCallback(() => {
    GetValueSpaces((data) => {
      if (data) {
        const values = Object.keys(data).map((k: string) => data[k]);
        setList(values);
      }
    });
  }, []);

  useEffect(() => {
    refreshTable();
  }, [refreshTable]);

  const columns = useMemo<MRT_ColumnDef<ValueSpacesProps>[]>(
    () => [
      {
        accessorKey: "name",
        header: "名称",
        size: 120,
        grow: false,
      },
      {
        accessorKey: "title",
        header: "标题",
        size: 120,
        grow: false,
      },
      {
        header: "可编辑",
        dataIndex: "configureLevel",
        size: 80,
        grow: false,
        Cell: ({ row }) => {
          return row.original.configureLevel === "system" ? "系统" : "可配置";
        },
      },
      {
        accessorKey: "conditions",
        header: "内容",
        Cell: ({ row }) => {
          const val = row.original.conditions;
          if (typeof val === "string") {
            return val;
          }
          const items = Object.keys(val)
            .sort()
            .slice(0, 5)
            .map((x, i) => (
              <span key={x}>
                {i > 0 && "; "}
                {row.original.valueSpaceType === 'Code' ? `${x}: ${val[x]}` : val[x]}
              </span>
            ));
          return <Box>{items}</Box>;
        },
        maxSize: 800, 
        minSize: 300,
      },
    ],
    []
  );

  const table = useMantineReactTable({
    columns,
    data: list,
    enableTopToolbar: false,
    enableRowActions: true,
    positionActionsColumn: "last",
    localization: MRT_Localization_ZH_HANS,
    displayColumnDefOptions: {
      "mrt-row-actions": {
        size: 48, //make actions column wider
      },
    },
    renderRowActions: ({ row }) => (
      <Box style={{ display: "flex", flexWrap: "nowrap", gap: "9px" }}>
        <ActionIcon
          variant="default"
          onClick={() => {
            const valueSpaceInfo: MenuItem = {
              uri: `/options/valueSpaces/${row.original.name}`,
              title: "编辑值空间",
            };
            addTab(valueSpaceInfo, "end");
          }}
        >
          <IconEdit stroke={1} />
        </ActionIcon>
      </Box>
    ),
    mantineTableProps: {
      withColumnBorders: true,
      highlightOnHoverColor: colors.blue[5],
    },
    layoutMode: "grid",
    mantineTableHeadCellProps: {
      // @ts-ignore
      sx: {
        flex: "0 0 auto",
      },
    },
    mantineTableBodyCellProps: {
      // @ts-ignore
      sx: {
        flex: "0 0 auto",
      },
    },
  });

  return (
    <Stack>
      <Title order={2}>值空间列表</Title>
      <MantineReactTable table={table} />
    </Stack>
  );
};

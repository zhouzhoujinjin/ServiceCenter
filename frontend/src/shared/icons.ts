import {
  IconChartBubble,
  IconTool,
  TablerIconsProps,
  IconBook,
  IconCalendar,
  IconCertificate,
  IconChartBar,
  IconChartDonut,
  IconChartInfographic,
  IconChartLine,
  IconChartPie,
  IconSettings,
  IconHammer,
  IconHome,
  IconIdBadge2,
  IconMedal,
  IconShield,
  IconSquare,
  IconUser,
  IconUsers,
  IconX,
} from "@tabler/icons-react";

export type BuiltinIconInfo = {
  key: string;
  title: string;
  icon: (props: TablerIconsProps) => JSX.Element;
};

export const BuiltinMenuIcons: BuiltinIconInfo[] = [
  {
    key: "empty",
    title: "主页",
    icon: IconSquare,
  },
  {
    key: "home",
    title: "主页",
    icon: IconHome,
  },
  {
    key: "department",
    title: "部门",
    icon: IconIdBadge2,
  },
  {
    key: "user",
    title: "用户",
    icon: IconUser,
  },
  {
    key: "role",
    title: "角色",
    icon: IconUsers,
  },
  {
    key: "settings",
    title: "设置",
    icon: IconSettings,
  },
  {
    key: "toolbox",
    title: "工具箱",
    icon: IconHammer,
  },
  {
    key: "wrench",
    title: "扳子",
    icon: IconTool,
  },
  {
    key: "calendar",
    title: "日历",
    icon: IconCalendar,
  },
  {
    key: "chart",
    title: "图表",
    icon: IconChartInfographic,
  },
  {
    key: "chart-bar",
    title: "柱图",
    icon: IconChartBar,
  },
  {
    key: "chart-line",
    title: "线图",
    icon: IconChartLine,
  },
  {
    key: "chart-donut",
    title: "环图",
    icon: IconChartDonut,
  },
  {
    key: "chart-pie",
    title: "饼图",
    icon: IconChartPie,
  },
  {
    key: "chart-scatter",
    title: "散点图",
    icon: IconChartBubble,
  },
  {
    key: "book",
    title: "图书",
    icon: IconBook,
  },
  {
    key: "shield",
    title: "盾牌",
    icon: IconShield,
  },
  {
    key: "medal",
    title: "勋章",
    icon: IconMedal,
  },
  {
    key: "certificate",
    title: "证书",
    icon: IconCertificate,
  },
  {
    key: "x",
    title: "删除",
    icon: IconX,
  },
];

export const BuiltinMenuIconMap: Record<string, BuiltinIconInfo> =
  BuiltinMenuIcons.reduce((pv: { [key: string]: BuiltinIconInfo }, c) => {
    pv[c.key] = c;
    return pv;
  }, {});

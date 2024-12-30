import {
  Avatar,
  Badge,
  Group,
  Menu,
  UnstyledButton,
  rem,
  useMantineColorScheme,
  useMantineTheme,
} from "@mantine/core";
import { createElement, useState } from "react";
import cx from "clsx";
import classes from "./UserButton.module.css";
import {
  IconMoon,
  IconSun,
  IconLogout,
  IconNotification,
  IconWand,
} from "@tabler/icons-react";
import { useAuth } from "~/hooks";

const SchemeIcon = () => {
  const { colorScheme } = useMantineColorScheme();
  const theme = useMantineTheme();

  const iconComp = colorScheme === "light" ? IconMoon : IconSun;
  return createElement(iconComp, {
    style: { width: rem(16), height: rem(16) },
    color:
      colorScheme === "light" ? theme.colors.blue[6] : theme.colors.yellow[6],
    stroke: 1.5,
  });
};

export const UserButton = () => {
  const theme = useMantineTheme();
  const { toggleColorScheme } = useMantineColorScheme();
  const [userMenuOpened, setUserMenuOpened] = useState(false);
  const [notificationsCount, setNotificationsCount] = useState(0);
  const { user } = useAuth();
  return (
    <Menu
      width={160}
      position="bottom-end"
      transitionProps={{ transition: "pop-top-right" }}
      onClose={() => setUserMenuOpened(false)}
      onOpen={() => setUserMenuOpened(true)}
      withinPortal
    >
      <Menu.Target>
        <UnstyledButton className={cx(classes.user)}>
          <Group gap={7}>
            <Avatar
              color={userMenuOpened ? theme.colors.blue[6] : undefined}
              src={user?.avatar}
              alt={user?.fullName || user?.userName}
              radius="xl"
              size={32}
            />
          </Group>
        </UnstyledButton>
      </Menu.Target>
      <Menu.Dropdown>
        <Menu.Item
          leftSection={
            <IconNotification
              style={{ width: rem(16), height: rem(16) }}
              color={theme.colors.red[6]}
              stroke={1.5}
            />
          }
          rightSection={
            notificationsCount ? <Badge>{notificationsCount}</Badge> : ""
          }
        >
          通知
        </Menu.Item>
        <Menu.Item
          leftSection={
            <IconWand
              style={{ width: rem(16), height: rem(16) }}
              color={theme.colors.lime[6]}
              stroke={1.5}
            />
          }
          onClick={() => {
            console.log('user', user)
          }}
        >
          个人设置1
        </Menu.Item>
        <Menu.Item
          leftSection={<SchemeIcon />}
          onClick={() => toggleColorScheme()}
        >
          切换主题
        </Menu.Item>
        <Menu.Divider />

        <Menu.Item
          leftSection={
            <IconLogout
              style={{ width: rem(16), height: rem(16) }}
              color={theme.colors.indigo[6]}
              stroke={1.5}
            />
          }
        >
          退出登录
        </Menu.Item>
      </Menu.Dropdown>
    </Menu>
  );
};

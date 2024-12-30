import {
  ActionIcon,
  Box,
  Burger,
  Menu,
  NavLink,
  Stack,
  Tooltip,
  rem,
  useMantineTheme,
} from "@mantine/core";
import React, { CSSProperties, FC, useEffect, useState } from "react";
import { BuiltinMenuIconMap } from "~/shared/icons";
import { MenuItem, WithParents } from "~/types";
import { loop } from "~/shared/utils";
import classes from "./FullMenu.module.css";
import { IconChevronRight } from "@tabler/icons-react";

interface MenuRenderProps {
  items: MenuItem[];
  activeKeyPath: string[];
  onChange?: (item: MenuItem) => void;
}

export type IconType = "builtin" | "image" | undefined;

const colors = [
  "blue",
  "pink",
  "teal",
  "red",
  "green",
  "cyan",
  "lime",
  "indigo",
  "violet",
  "grape",
];

export type FullMenuProps = {
  items: MenuItem[];
  variant?: "nav" | "collapsed-nav" | "button";
  activeKey: string;
  onChange?: (item: MenuItem) => void;
};

const ItemIcon = ({
  iconType,
  icon,
  color,
  style,
  size,
  stroke,
}: {
  iconType: IconType;
  icon?: string;
  color?: string;
  style?: CSSProperties;
  size?: number;
  stroke?: number;
}) => {
  if (!icon) {
    return null;
  }
  switch (iconType) {
    case "image":
      return <img src={icon} className={classes.imageIcon} style={style} />;
    default: {
      const IconComp = BuiltinMenuIconMap[icon]
        ? BuiltinMenuIconMap[icon].icon
        : BuiltinMenuIconMap.empty.icon;
      return React.createElement(IconComp, { color, style, size, stroke });
    }
  }
};

export const FullMenu = ({
  items,
  activeKey,
  variant,
  onChange,
}: FullMenuProps) => {
  const [menuItems, setMenuItems] = useState(
    new Map<string, WithParents<MenuItem>>()
  );
  const [activeKeyPath, setActiveKeyPath] = useState<string[]>([]);

  const handleChange = (item: WithParents<MenuItem>) => {
    let path: string[] = [];
    if (!item.childrenInvisible && item.children && item.children.length > 0) {
      path = item.parents!.map((x) => x.uri);
    }
    path.push(item.uri);
    setActiveKeyPath(path);
    if (item.type === "route" && activeKey !== item.uri) {
      onChange && onChange(item);
    }
  };

  useEffect(() => {
    const map = new Map<string, WithParents<MenuItem>>();
    loop<WithParents<MenuItem>>(items, (item, _index, _array, parent) => {
      map.set(item.uri, item);
      if (!parent) {
        item.parents = [];
      } else {
        item.parents =
          !parent.parents || parent.parents?.length == 0
            ? [parent]
            : [...parent.parents!, parent];
      }
    });
    setMenuItems(map);
  }, [items]);

  useEffect(() => {
    if (menuItems.has(activeKey)) {
      const parentKeys = menuItems.get(activeKey)!.parents!.map((x) => x.uri);
      setActiveKeyPath(parentKeys);
    }
  }, [menuItems, activeKey]);

  switch (variant) {
    case "button":
      return (
        <ButtonMenuRender
          items={items}
          activeKeyPath={activeKeyPath}
          onChange={handleChange}
        />
      );
    case "collapsed-nav":
      return (
        <CollapsedNavRender
          items={items}
          activeKeyPath={activeKeyPath}
          onChange={handleChange}
        />
      );
    default:
      return (
        <NavRender
          items={items}
          activeKeyPath={activeKeyPath}
          onChange={handleChange}
        />
      );
  }
};

const NavRender: FC<MenuRenderProps> = ({ items, activeKeyPath, onChange }) => {
  const theme = useMantineTheme();
  return items.map((x, index) => (
    <NavLink
      key={x.uri}
      leftSection={
        <ItemIcon
          color={theme.colors[colors[index]][4]}
          iconType={x.iconType}
          icon={x.icon}
          stroke={1.5}
        />
      }
      label={x.title}
      onClick={(e) => {
        e.preventDefault();
        if (x.type === "route") {
          onChange && onChange(x);
        }
      }}
    >
      {x.children && x.children.length > 0 && !x.childrenInvisible && (
        <NavRender
          items={x.children}
          activeKeyPath={activeKeyPath}
          onChange={onChange}
        />
      )}
    </NavLink>
  ));
};

const ButtonMenuRender: FC<MenuRenderProps> = ({
  items,
  activeKeyPath,
  onChange,
}) => {
  const [opened, setOpened] = useState(false);
  const renderMenu = (items: MenuItem[], activeKeyPath: string[]) => {
    return items.map((item) =>
      item.children ? (
        <Menu.Item
          closeMenuOnClick={false}
          className={
            activeKeyPath.includes(item.uri) ? classes.active : undefined
          }
          leftSection={
            <ItemIcon
              iconType={item.iconType}
              icon={item.icon}
              style={{ width: rem(14), height: rem(14) }}
            />
          }
          rightSection={
            <IconChevronRight style={{ width: rem(14), height: rem(14) }} />
          }
        >
          <Menu
            position="right-start"
            trigger="hover"
            offset={{ mainAxis: 20, crossAxis: -10 }}
          >
            <Menu.Target>
              <Box>{item.title}</Box>
            </Menu.Target>

            <Menu.Dropdown>
              {renderMenu(item.children, activeKeyPath)}
            </Menu.Dropdown>
          </Menu>
        </Menu.Item>
      ) : (
        <Menu.Item
          onClick={() => {
            setOpened(false);
            onChange && onChange(item);
          }}
        >
          <Box>{item.title}</Box>
        </Menu.Item>
      )
    );
  };

  return (
    <Menu opened={opened} onChange={setOpened}>
      <Menu.Target>
        <Burger opened={opened} onChange={() => setOpened((o) => !o)} />
      </Menu.Target>

      <Menu.Dropdown>{renderMenu(items, activeKeyPath)}</Menu.Dropdown>
    </Menu>
  );
};

const CollapsedNavRender: FC<MenuRenderProps> = ({
  items,
  activeKeyPath,
  onChange,
}) => {
  const [openedKey, setOpenedKey] = useState("");
  const theme = useMantineTheme();

  const renderMenu = (items: MenuItem[], activeKeyPath: string[]) => {
    return items.map((item) =>
      !item.childrenInvisible && item.children && item.children.length ? (
        <Menu.Item
          key={item.uri}
          closeMenuOnClick={false}
          className={
            activeKeyPath.includes(item.uri) ? classes.active : undefined
          }
          leftSection={
            <ItemIcon
              iconType={item.iconType}
              icon={item.icon}
              style={{ width: rem(14), height: rem(14) }}
            />
          }
          rightSection={
            <IconChevronRight style={{ width: rem(14), height: rem(14) }} />
          }
        >
          <Menu
            width={140}
            position="right-start"
            trigger="hover"
            offset={{ mainAxis: 20, crossAxis: -10 }}
          >
            <Menu.Target>
              <Box>{item.title}</Box>
            </Menu.Target>

            <Menu.Dropdown>
              {renderMenu(item.children, activeKeyPath)}
            </Menu.Dropdown>
          </Menu>
        </Menu.Item>
      ) : (
        <Menu.Item
          key={item.uri}
          onClick={() => {
            onChange && onChange(item);
          }}
        >
          {item.title}
        </Menu.Item>
      )
    );
  };

  return (
    <Stack gap="md" align="center">
      {items.map((item, index) =>
        item.children && item.children.length && !item.childrenInvisible ? (
          <Menu
            width={140}
            key={item.uri}
            position="right-start"
            offset={{ mainAxis: 8 }}
            opened={openedKey === item.uri}
          >
            <Menu.Target>
              <ActionIcon
                size="xl"
                variant={activeKeyPath.includes(item.uri) ? "filled" : "subtle"}
                onMouseMove={() => setOpenedKey(item.uri)}
                color={theme.colors[colors[index]][4]}
              >
                <ItemIcon
                  size={32}
                  stroke={1.5}
                  iconType={item.iconType}
                  icon={item.icon}
                />
              </ActionIcon>
            </Menu.Target>
            <Menu.Dropdown>
              <Menu.Label>{item.title}</Menu.Label>
              {renderMenu(item.children, activeKeyPath)}
            </Menu.Dropdown>
          </Menu>
        ) : (
          <Tooltip position="right" label={item.title} key={item.uri}>
            <ActionIcon
              size="xl"
              color={theme.colors[colors[index]][4]}
              variant={activeKeyPath.includes(item.uri) ? "filled" : "subtle"}
              onClick={() => onChange && onChange(item)}
            >
              <ItemIcon
                iconType={item.iconType}
                icon={item.icon}
                size={32}
                stroke={1.5}
              />
            </ActionIcon>
          </Tooltip>
        )
      )}
    </Stack>
  );
};

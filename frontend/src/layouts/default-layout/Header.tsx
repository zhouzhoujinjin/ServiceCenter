import { ActionIcon, Group } from "@mantine/core";
import { IconChevronsLeft, IconChevronsRight } from "@tabler/icons-react";
import classes from "./Header.module.css";
import { useSystemOptions } from "~/hooks/useSystemOptions";
import { UserButton } from "./UserButton";
import { TagTabs } from "~/components/tag-tabs/TagTabs";
import { useTabLayout } from "~/hooks";
export const Header = () => {
  const {
    platformOptions: { siteTitle },
    uiOptions: { siderCollapsed },
    updateSiderCollapsed,
  } = useSystemOptions();
  const { activePath, items, setActive, removeTab } = useTabLayout();

  return (
    <Group className={classes.header}>
      <div className={classes.leftSection}>
        <ActionIcon
          variant="subtle"
          onClick={() => updateSiderCollapsed(!siderCollapsed)}
        >
          {siderCollapsed ? <IconChevronsRight /> : <IconChevronsLeft />}
        </ActionIcon>
        <h2>{siteTitle || "市场中心"}</h2>
      </div>
      <div className={classes.middleSection}>
        <TagTabs
          activeId={activePath}
          items={items}
          onTabChange={(id) => {
            setActive(id);
          }}
          onTabRemove={(id) => removeTab(id)}
        />
      </div>
      <div className={classes.rightSection}>
        <UserButton />
      </div>
    </Group>
  );
};

import { FC, useState } from "react";

import clsx from "clsx";
import { ScrollArea } from "@mantine/core";
import { ScrollInfo, useHorizontalScroll } from "~/hooks/useHorizontalScroll";
import classes from "./TagTabs.module.css";
import { TagButton } from "./TagButton";

export interface TagTabItem {
  uri: string;
  title: string;
}
export type TagTabsProps = {
  items: TagTabItem[];
  activeId?: string;
  onTabChange?: (itemId: string) => void;
  onTabRemove?: (itemId: string) => void;
};

export const TagTabs: FC<TagTabsProps> = ({
  items,
  activeId,
  onTabChange,
  onTabRemove,
}) => {
  const [scrollInfo, setScrollInfo] = useState<ScrollInfo>();  
  const tabsContainerRef = useHorizontalScroll((data) => setScrollInfo(data));

  return (
    <ScrollArea
      scrollbars="x"
      scrollbarSize={2}
      h={60}
      viewportRef={tabsContainerRef}
      classNames={{
        viewport: clsx(
          classes.viewport,
          scrollInfo?.atEnd === false && classes.atEnd,
          scrollInfo?.atStart === false && classes.atStart
        ),
      }}
    >
      <div className={classes.root}>
        {items &&
          items.map((item) => {
            return (
              <TagButton
                key={item.uri}
                active={activeId === item.uri}
                onClick={() => onTabChange && onTabChange(item.uri)}
                onClose={() => onTabRemove && onTabRemove(item.uri)}
              >
                {item.title}
              </TagButton>
            );
          })}
      </div>
    </ScrollArea>
  );
};

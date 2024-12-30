import React, { forwardRef, HTMLAttributes, ReactNode } from "react";
import styles from "./TreeItem.module.css";
import { ActionIcon } from "@mantine/core";
import {
  IconChevronDown,
  IconChevronRight,
  IconGripVertical,
} from "@tabler/icons-react";
import clsx from "clsx";

export interface Props extends Omit<HTMLAttributes<HTMLDivElement>, "id"> {
  clone?: boolean;
  collapsed?: boolean;
  depth: number;
  disableInteraction?: boolean;
  disableSelection?: boolean;
  ghost?: boolean;
  active?: boolean;
  handleProps?: any;
  indicator?: boolean;
  indentationWidth: number;
  value: ReactNode;
  onCollapse?(): void;
  onContentClick?(): void;
  rightSection?: ReactNode;

  wrapperRef?(node: HTMLDivElement): void;
}

export const TreeItem = forwardRef<HTMLDivElement, Props>(
  (
    {
      clone,
      depth,
      disableSelection,
      disableInteraction,
      ghost,
      indentationWidth,
      indicator,
      collapsed,
      onCollapse,
      rightSection,
      style,
      value,
      handleProps,
      wrapperRef,
      onContentClick,
      active,
      ...props
    },
    ref
  ) => {
    return (
      <div
        className={clsx(
          styles.treeItemRoot,
          clone && styles.clone,
          ghost && styles.ghost,
          indicator && styles.indicator,
          disableSelection && styles.disableSelection,
          disableInteraction && styles.disableInteraction
        )}
        ref={wrapperRef}
        style={
          {
            "--spacing": `${indentationWidth * depth}px`,
          } as React.CSSProperties
        }
        {...props}
      >
        <div className={clsx(styles.treeItem, active && styles.treeItemActive)} ref={ref} style={style}>
          <ActionIcon
            variant="subtle"
            className={styles.dragHandle}
            {...handleProps}
          >
            <IconGripVertical stroke={1} />
          </ActionIcon>
          {onCollapse ? (
            <ActionIcon
              className={styles.collapseHandle}
              onClick={onCollapse}
              variant="subtle"
            >
              {collapsed ? (
                <IconChevronRight width={24} />
              ) : (
                <IconChevronDown width={24} />
              )}
            </ActionIcon>
          ) : (
            <div style={{ width: 24 }}></div>
          )}
          <div
            className={styles.treeItemContent}
            onClick={() => onContentClick && onContentClick()}
          >
            {value}
          </div>
          {!clone && rightSection}
        </div>
      </div>
    );
  }
);

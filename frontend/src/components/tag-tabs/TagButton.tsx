import { Button } from "@mantine/core";
import { IconX } from "@tabler/icons-react";
import { FC, PropsWithChildren } from "react";
import classes from "./TagButton.module.css";
import clsx from "clsx";

export const TagButton: FC<
  PropsWithChildren<{
    active?: boolean;
    onClick?: () => void;
    onClose?: () => void;
  }>
> = ({ children, active, onClose, onClick }) => {
  return (
    <Button
      variant={active ? "primary" : "default"}
      fz={14}
      fw={500}
      h={28}
      className={classes.root}
      size="compact-sm"
      rightSection={
        <div
          className={clsx(classes.closeIcon, active && classes.closeIconActive)}
        >
          <IconX
            onClick={(e) => {
              e.stopPropagation();
              e.preventDefault();
              onClose && onClose();
            }}
            style={{
              width: 16,
              height: 16,
            }}
          />
        </div>
      }
      onClick={(e) => {
        e.stopPropagation();
        e.preventDefault();
        onClick && onClick();
      }}
    >
      {children}
    </Button>
  );
};

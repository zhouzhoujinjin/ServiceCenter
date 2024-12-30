import {
  ActionIcon,
  Box,
  Button,
  FileButton,
  Input,
  InputWrapperProps,
  Tooltip,
} from "@mantine/core";
import { IconTrash, IconUpload } from "@tabler/icons-react";
import { isEqual } from "lodash-es";
import { useEffect, useState } from "react";
import classes from "./SingleImageUploader.module.css";
import { CustomFile } from "./types";

type SingleImageUploadProps = Omit<
  InputWrapperProps,
  "children" | "onChange"
> & {
  value?: string | { url: string };
  uploadAction: (file: File) => Promise<string>;
  onChange?: (value: string | null) => void;
  previewWidth?: number;
  maxSize?: number;
  aspectRatio?: number;
  children?: React.ReactNode;
};

export function SingleImageUpload({
  value,
  uploadAction,
  label,
  onChange,
  previewWidth = 120,
  aspectRatio,
  children,
  ...props
}: SingleImageUploadProps) {
  const [image, setImage] = useState<CustomFile | undefined>();
  const [error, setError] = useState("");

  const handleChange = async (file: File | null) => {
    if (!file) {
      return;
    }
    setError("");
    const toUpload = { url: URL.createObjectURL(file), file };
    setImage((current) => ({
      ...current,
      previewUrl: toUpload.url,
      url: "",
      file: toUpload.file,
    }));

    const url = await uploadAction(toUpload.file);
    setImage((current) => ({
      ...current,
      url: url,
      file: undefined,
      previewUrl: undefined,
    }));
    URL.revokeObjectURL(toUpload.url);
    onChange && onChange(url);
  };

  const handleRemove = () => {
    setImage(undefined);
    onChange?.(null);
  };

  useEffect(() => {
    const newValue =
      typeof value === "string"
        ? value.length > 0
          ? { url: value }
          : undefined
        : value;

    if (!isEqual(image, newValue))
      setImage((current) =>
        typeof value === "string"
          ? value.length > 0
            ? { url: value }
            : current
          : current
      );
  }, [image, value]);
  // console.log(image);
  return (
    <Input.Wrapper {...props} error={props.error ?? error}>
      {label && <Input.Label>{label}</Input.Label>}
      {image?.previewUrl || image?.url ? (
        <div className={classes.imageWrapper} style={{ width: previewWidth + 2 }}>
          <Tooltip label="删除图片">
            <ActionIcon
              size="md"
              variant={aspectRatio ? "filled" : "light"}
              color="red"
              onClick={handleRemove}
              className={classes.removeButton}
            >
              <IconTrash />
            </ActionIcon>
          </Tooltip>

          <Box w={120} h={120}>
            <img
              src={image.previewUrl ?? image.url}
              width={previewWidth}
              style={{
                maxWidth: aspectRatio ? "100%" : undefined,
                borderRadius: 5,
              }}
            />
          </Box>
        </div>
      ) : (
        <FileButton
          accept="image/png,image/jpeg"
          onChange={(file) => {
            handleChange(file).then(() => {

            });
          }}
        >
          {(ps) => (
            <Button
              variant="default"
              leftSection={<IconUpload />}
              fullWidth
              {...ps}
            >
              上传图片
            </Button>
          )}
        </FileButton>
      )}
      {children}
    </Input.Wrapper>
  );
}

import { FileWithPath } from "@mantine/dropzone";

export type CustomFile = {
  id?: number;
  url: string;
  previewUrl?: string;
  onLoad?: () => void;
  name?: string;
  meta?: Record<string, unknown> | null;
  file?: FileWithPath;
  height?: number | null;
  width?: number | null;
  hash?: string;
  tags?: Array<{ id: number; name: string; isCategory: boolean }>;
  uuid?: string;
  status?: 'processing' | 'uploading' | 'complete' | 'blocked' | 'error';
  blockedFor?: string[];
  message?: string;
};

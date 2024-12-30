import { FC, useEffect, useState } from "react";
import { UserData } from "./types";
import { useForm } from "@mantine/form";
import { Button, Group, TextInput } from "@mantine/core";
import { DatePickerInput } from "@mantine/dates";
import "@mantine/dates/styles.css";
import { CreateUser, Pinyin, UpdateAdminUser } from "./services/user";
import dayjs from "dayjs";
import { cloneDeep } from "lodash-es";
import { SingleImageUpload } from "~/components/single-image-uploader/SingleImageUploader";
import { api } from "~/shared";

export const UserInfo: FC<{
  userData?: UserData;
  onSave?: (values: any) => void;
}> = ({ userData, onSave }) => {
  const [submitType, setSubmitType] = useState<string>("create");

  const form = useForm<UserData>({
    initialValues: userData || {
      id: undefined,
      userName: "",
      profiles: {
        fullName: "",
        birthday: new Date(),
      },
      isVisible: true,
    },
  });

  const onFormSubmit = (values: UserData) => {
    if (values) {
      const postValues = cloneDeep(values);
      console.log("value", values);
      console.log("clone", postValues);
      if (postValues.profiles?.birthday) {
        postValues.profiles.birthday = dayjs(
          postValues.profiles.birthday
        ).format("YYYY-MM-DD");
      }
      if (submitType === "update") {
        UpdateAdminUser(postValues.userName, postValues, (data) => {
          onSave && onSave(data);
        });
      } else {
        CreateUser(postValues, (data) => {
          onSave && onSave(data);
        });
      }
    }
  };

  useEffect(() => {
    if (userData && userData.id) {
      setSubmitType("update");
    }
  }, [userData]);

  return (
    <form
      onSubmit={form.onSubmit((values) => {
        onFormSubmit(values);
      })}
    >
      <TextInput
        withAsterisk
        label="真实姓名"
        onBlurCapture={(v) => {
          if (submitType === 'create') {
            if (v.target.value) {
              Pinyin(v.target.value, (pinyin) => {
                if (pinyin) {
                  form.setFieldValue('userName', pinyin);
                }
              })
            }
          }
        }}
        mb='md'
        {...form.getInputProps("profiles.fullName")}
      />
      <TextInput
        withAsterisk
        label="用户登录名"
        disabled={submitType === "update"}
        mb='md'
        {...form.getInputProps("userName")}
      />
      <DatePickerInput
        label="生日"
        placeholder="Date input"
        valueFormat="YYYY-MM-DD"
        mb='md'
        {...form.getInputProps("profiles.birthday")}
      />

      <SingleImageUpload
        label="头像"
        uploadAction={async (file) => {
          const result = await api.file<string>(`/api/admin/users/avatar`, {
            avatar: file,
          });
          return result as string;
        }}
        {...form.getInputProps("profiles.avatar")}
      ></SingleImageUpload>

      <Group justify="flex-end" mt="md">
        <Button type="submit">
          {submitType === "update" ? "保存" : "新增"}
        </Button>
      </Group>
    </form>
  );
};

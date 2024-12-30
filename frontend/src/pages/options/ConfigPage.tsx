import { useEffect, useState } from "react"
import { SystemConfig } from "~/types"
import { GetSystemConfig, UpdateSystemConfig } from "./services/config"
import { useForm } from "@mantine/form"
import { Button, Group, NumberInput, Stack, TextInput } from "@mantine/core"
import { SingleImageUpload } from "~/components/single-image-uploader"
import { api } from "~/shared"

export const ConfigPage = () => {

  const [sysConfig, setSysConfig] = useState<SystemConfig>()

  const form = useForm<SystemConfig>({
    initialValues: sysConfig || {
      openEndDate: new Date(),
      openStartDate: new Date(),
      // siteUnderMaintenance: "",
      siteName: "",
      defaultAvatarSize: { width: 30, height: 30 },
      logoUri: "",
      beiAnNO: "",
      SiteUnderMaintenance: true
    },
  });

  useEffect(() => {
    GetSystemConfig(data => {
      if (data) {
        console.log(data)
        setSysConfig(data)
        form.setValues(data)
      }
    })
  }, [])

  const onFormSubmit = (values: SystemConfig) => {
    UpdateSystemConfig(values)
  }


  return (
    <form
      onSubmit={form.onSubmit((values) => {
        onFormSubmit(values);
      })}
    >
      <Stack w="600">
        <TextInput
          withAsterisk
          label="网站标题"
          mb='md'
          {...form.getInputProps("siteName")}
        />
        {/* <NumberInput
          withAsterisk
          label="头像尺寸(最小30*30)"
          min={30}
          max={60}
          mb='md'
          {...form.getInputProps("defaultAvatarSize")}
        /> */}
        <TextInput
          label="网站备案号"
          mb='md'
          {...form.getInputProps("beiAnNO")}
        />
        <SingleImageUpload
          label="网站logo"
          uploadAction={async (file) => {
            const result = await api.file<string>(`/api/admin/users/avatar`, {
              avatar: file,
            });
            return result as string;
          }}
          {...form.getInputProps("logoUri")}
        ></SingleImageUpload>

        <Group justify="flex-end" mt="md">
          <Button type="submit">
            保存
          </Button>
        </Group>
      </Stack>
    </form>

  )


}
import {
  ActionIcon,
  Anchor,
  Box,
  Button,
  Card,
  Divider,
  Group,
  PasswordInput,
  TextInput,
  rem,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { IconChevronLeft } from "@tabler/icons-react";
import { Link } from "react-router-dom";

export const ForgotPasswordPage = () => {
  const form = useForm({
    initialValues: {
      userName: "",
      newPassword: "",
      phoneNumber: "",
      verifyCode: "",
    },

    validate: {
      userName: (val) => (/^[a-zA-z0-9_-]+$/.test(val) ? null : "无效的用户名"),
      newPassword: (val) =>
        val.length <= 6
          ? "Password should include at least 6 characters"
          : null,
    },
  });
  return (
    <Box w={360} mx="auto">
      <Group>
        <ActionIcon variant="subtle" component={Link} to="../login">
          <IconChevronLeft />
        </ActionIcon>
        <h4 style={{ margin: "0px", lineHeight: '1rem' }}>重设密码</h4>
      </Group>
      <Card.Section>
        <Divider my={rem(20)} />
      </Card.Section>
      <form onSubmit={form.onSubmit((values) => console.log(values))}>
        <TextInput
          label="用户名"
          placeholder="请输入用户名"
          {...form.getInputProps("userName")}
        />
        <TextInput
          mt="md"
          label="手机号码"
          placeholder="请输入系统绑定的手机号码"
          {...form.getInputProps("phoneNumber")}
        />
        <TextInput
          mt="md"
          label="验证码"
          placeholder="请输入手机收到的验证码"
          rightSectionWidth={120}
          rightSection={
            <Button
              styles={{
                root: {
                  borderTopLeftRadius: 0,
                  borderBottomLeftRadius: 0,
                },
              }}
              fullWidth
              variant="light"
            >
              发送验证码
            </Button>
          }
          {...form.getInputProps("verfiyCode")}
        />
        <PasswordInput
          mt="md"
          label="新密码"
          placeholder="请输入密码"
          {...form.getInputProps("newPassword")}
        />
        <Group justify="space-between" mt="xl">
          <Button type="submit">修改密码</Button>
        </Group>
      </form>
    </Box>
  );
};

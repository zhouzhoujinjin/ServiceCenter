import {
  Anchor,
  Box,
  Button,
  Card,
  Checkbox,
  Divider,
  Group,
  PasswordInput,
  TextInput,
  Tooltip,
  rem,
} from "@mantine/core";
import { useForm } from "@mantine/form";
import { Link, useNavigate } from "react-router-dom";
import { LoginParams } from "./types";
import { FetchCaptchaImage, Login } from "./services/account";
import { token } from "~/shared/token";
import { useEffect, useState } from "react";

const enableCaptcha = import.meta.env.VITE_ENABLE_CAPTCHA === "true";

export const LoginPage = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const form = useForm<LoginParams>({
    initialValues: {
      userName: "",
      password: "",
      rememberMe: true,
      captchaCode: "",
      captchaId: "",
    },

    validate: {
      userName: (val) => (/^[a-zA-z0-9_-]+$/.test(val) ? null : "无效的用户名"),
      password: (val) =>
        val.length < 6 ? "Password should include at least 6 characters" : null,
    },
  });

  const [imageData, setImageData] = useState("");

  useEffect(() => {
    if (enableCaptcha) {
      FetchCaptchaImage((data) => {
        form.setFieldValue("captchaId", data.id);
        setImageData(data.content);
      });
    }
  }, []);

  const handleLogin = (values: LoginParams) => {
    setLoading(true);
    const id = setTimeout(() => setLoading(false), 5000);
    Login(values, (data) => {
      if (data && data.accessToken) {
        token.save(data.accessToken);
        navigate("/");
      } else {
        FetchCaptchaImage((data) => {
          form.setFieldValue("captchaId", data.id);
          setImageData(data.content);
        });
      }
      clearTimeout(id);
      setLoading(false);
    });
  };

  return (
    <Box w={360} mx="auto">
      <h4 style={{ margin: 0 }}>登录</h4>
      <Card.Section>
        <Divider my={rem(20)} />
      </Card.Section>
      <form onSubmit={form.onSubmit((values) => handleLogin(values))}>
        <TextInput
          label="用户名"
          placeholder="请输入用户名"
          {...form.getInputProps("userName")}
        />
        <PasswordInput
          mt="md"
          label="密码"
          placeholder="请输入密码"
          {...form.getInputProps("password")}
        />
        {enableCaptcha && (
          <TextInput
            mt="md"
            label="验证码"
            placeholder="请输入右侧图片中的文字"
            rightSectionWidth={120}
            rightSection={
              <Tooltip label="点击刷新">
                <Button
                  styles={{
                    root: {
                      borderTopLeftRadius: 0,
                      borderBottomLeftRadius: 0,
                      padding: 0,
                    },
                  }}
                  fullWidth
                  variant="light"
                  onClick={() =>
                    FetchCaptchaImage((data) => {
                      form.setFieldValue("captchaId", data.id);
                      setImageData(data.content);
                    })
                  }
                >
                  <img
                    src={imageData}
                    style={{
                      height: "100%",
                    }}
                  />
                </Button>
              </Tooltip>
            }
            {...form.getInputProps("captchaCode")}
          />
        )}
        <Checkbox
          label="记住我"
          mt="md"
          {...form.getInputProps("rememberMe", { type: "checkbox" })}
        ></Checkbox>
        <Group justify="space-between" mt="xl">
          <Anchor component={Link} to="../forgot-password">
            忘记密码
          </Anchor>
          <Button
            type="submit"
            w={rem(200)}
            loading={loading}
            disabled={loading}
          >
            登录
          </Button>
        </Group>
      </form>
    </Box>
  );
};

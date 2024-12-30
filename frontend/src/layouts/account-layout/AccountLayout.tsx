import { Link, Outlet, Route, Routes, useLocation } from "react-router-dom";
import classes from "./AccountLayout.module.css";
import { Anchor, Box, Card, Text, Stack } from "@mantine/core";
import { useSystemOptions } from "~/hooks";
import { ForgotPasswordPage, LoginPage } from "~/pages/account";
import { useEffect, useRef } from "react";

export const AccountLayout = () => {
  const {
    platformOptions: { siteTitle },
  } = useSystemOptions();
  const ref = useRef<HTMLDivElement | null>(null);
  const location = useLocation();

  useEffect(() => {
    if (ref.current) {
      ref.current.style.height =
        ref.current.children[0]?.clientHeight + 40 + "px";
    }
  }, [location]);

  return (
    <>
      <div className={classes.root}>
        <Box mb="xl" fz="xl">
          欢迎使用 {siteTitle || "PureCode Admin (v7)"}
        </Box>
        <Card
          shadow="md"
          withBorder
          px="xl"
          py="lg"
          style={{ transition: "height .3s ease-in", justifyContent: "center" }}
          ref={ref}
        >
          <Routes>
            <Route path="login" element={<LoginPage />} />
            <Route path="forgot-password" element={<ForgotPasswordPage />} />
            <Route
              path="*"
              element={
                <Stack align="center">
                  <div>
                    您访问的地址 [<strong>{location.pathname}</strong>] 并不存在
                  </div>
                  <div>
                    尝试返回{" "}
                    <Anchor component={Link} to="/index">
                      首页
                    </Anchor>
                  </div>
                </Stack>
              }
            />
          </Routes>
          <Outlet />
        </Card>
      </div>

      <footer className={classes.footer}>
        <Text c="dimmed" size="sm">
          © 2024 pure.codes. All rights reserved.
        </Text>
      </footer>
    </>
  );
};

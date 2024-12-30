import { AuthProvider } from "~/hooks";
import { Header } from "./Header";

import { Group, Stack } from "@mantine/core";
import { Sidebar } from "./Sidebar";
import { MainContent } from "./MainContent";
import { TabLayoutProvider } from "~/hooks";

import classes from './DefaultLayout.module.css'

export const DefaultLayout = () => {
  return (
    <AuthProvider loginRoute="/account/login">
      <TabLayoutProvider>
        <Stack gap={0} h="100%">
          <Header />
          <Group align="flex-start" className={classes.wrapper}>
            <Sidebar />
            <MainContent />
          </Group>
        </Stack>
      </TabLayoutProvider>
    </AuthProvider>
  );
};

import { MantineProvider } from "@mantine/core";
import { Notifications } from "@mantine/notifications";
import ReactDOM from "react-dom/client";

import App from "./App.tsx";
import { SystemOptionsProvider } from "./hooks/useSystemOptions.tsx";

import "@mantine/core/styles.css";
import "@mantine/notifications/styles.css";
import "./main.css";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <MantineProvider
    theme={{
      spacing: {
        xxs: "2px",
        xs: "4px",
        sm: "12px",
        md: "16px",
        lg: "20px",
        xl: "24px",
      },
    }}
  >
    <Notifications position="top-center" limit={1} w="auto" />
    <SystemOptionsProvider>
      <App />
    </SystemOptionsProvider>
  </MantineProvider>
);

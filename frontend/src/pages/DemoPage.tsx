import { TextInput } from "@mantine/core";
import { useLocation } from "react-router-dom";

export const DemoPage = () => {
  const location = useLocation();
  return (
    <div>
      {location.pathname} <TextInput />
    </div>
  );
};

export interface ValueSpacesProps {
  conditions: any;
  conditionArray?: ConditionArray[];
  title?: string;
  name: string;
  valueSpaceType: "Code" | "Range" | "Regex";
  configureLevel: string;
}

export interface ConditionArray {
  code: string;
  value: any;
}

export type ValueSpacesItem = Record<string, ValueSpacesProps>;

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Input, Button, Form } from "antd";
import {
  formItemLayout,
  formItemLayoutWithoutLabel,
} from "../../../../utils/formLayouts";
import { useEffect } from "react";
import { useState } from "react";
import { Select } from "antd";

import './styles.less'

const InputGroup = Input.Group;
const FormItem = Form.Item;

const TitleValueInput = (props) => {
  const { value, onRemove, key, removable, isFirst, index } = props;
  const triggerChange = (changedValue) => {
    const { onChange, value } = props;
    if (onChange) {
      onChange({
        ...value,
        ...changedValue,
      });
    }
  };

  const handleTitleChange = (e) => {
    triggerChange({ title: e.target.value });
  };

  const handleFapiaoChange = (v) => {
    triggerChange({ fapiao: v });
  };

  const handleValueChange = (e) => {
    triggerChange({ value: e.target.value });
  };

  const handleRemove = (e) => {
    onRemove(key);
  };

  return (
    <div className={`title-value-input ${isFirst ? "first" : ""}`}>
      <InputGroup compact>
        <Input
          addonBefore={`${index+1}. `}
          placeholder="标题"
          autoComplete="off"
          value={value.title}
          onChange={handleTitleChange}
        />
        <Select placeholder='发票类型' value={value.fapiao} onChange={handleFapiaoChange}>
          <Select.Option value='普票'>普票</Select.Option>
          <Select.Option value='专票'>专票</Select.Option>
        </Select>
        <Input
          placeholder="值"
          autoComplete="off"
          value={value.value}
          onChange={handleValueChange}
        />
        {removable ? (
          <Button onClick={handleRemove}>
            <FontAwesomeIcon icon="minus" />
          </Button>
        ) : (
          <Button disabled>
            <FontAwesomeIcon icon="minus" />
          </Button>
        )}
      </InputGroup>
    </div>
  );
};

const TitleValueList = (props) => {
  const [internalValue, setInternalValue] = useState(props.value || []);
  useEffect(() => {
    setInternalValue(props.value || []);
  }, [props.value]);

  return (
    <Form.List name={props.name}>
      {(fields, { add, remove }) => {
        return (
          <>
            {fields.map((field, index) => (
              <Form.Item
                className="form-item-narrow"
                {...field}
                {...(index === 0 ? formItemLayout : formItemLayoutWithoutLabel)}
                label={index === 0 ? props.label : null}
                key={field.key}
                validateTrigger={["onChange", "onBlur"]}
                rules={[{ required: true, message: "请输入选项" }]}
              >
                <TitleValueInput
                  index={index}
                  removable={internalValue.length > 1}
                  isFirst={index === 0}
                  onRemove={() => {
                    setInternalValue(f => {
                      const newF = [...f]
                      newF.splice(index, 1)
                      return newF
                    })
                     remove(field.name)
                  }}
                  onChange={(v) => {
                    setInternalValue((f) => {
                      const newF = [...f];
                      newF[index] = v;
                      return newF;
                    });
                  }}
                />
              </Form.Item>
            ))}
            <FormItem
              label={fields.length === 0 ? props.label : null}
              {...(fields.length === 0
                ? formItemLayout
                : formItemLayoutWithoutLabel)}
            >
              <div
                style={{
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                }}
              >
                <Button
                  type="dashed"
                  onClick={() => {
                    add({ title: "", value: "", fapiao:'普票' });
                  }}
                  block
                >
                  <FontAwesomeIcon icon="plus" /> 添加选项
                </Button>
                {props.options.showSummary && (
                  <span style={{ whiteSpace: "nowrap", marginLeft: 24 }}>
                    合计:{" "}
                    {internalValue.reduce((s, x) => {
                      s += parseFloat(x.value);
                      return s;
                    }, 0)}{" "}
                    {props.options.suffix || ""}
                  </span>
                )}
              </div>
            </FormItem>
          </>
        );
      }}
    </Form.List>
  );
};

export default TitleValueList;

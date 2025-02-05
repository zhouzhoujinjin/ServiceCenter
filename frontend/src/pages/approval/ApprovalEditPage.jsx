import React, { useState, useEffect } from "react";
import { PageWrapper } from "../../components/PageWrapper";
import { Form, Button, Card, message, Input } from "antd";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useHistory, useParams } from "react-router-dom";
import { useTabLayout } from "../../hooks/useTabLayout";
import ApprovalFlow from "./components/ApprovalFlow/index";
import {
  formItemLayout,
  formItemLayoutWithoutLabel,
} from "../../utils/formLayouts";
import {
  GetTemplate,
  GetDeptUsers,
  GetDepartments,
  CreateApprovalItem,
  GetApprovalItem,
  UpdateApprovalItem,
  PreviewFlow,
  GetDepartmentIds,
} from "./services/approval";
import { dotKeyToNested, nestedToDotKey } from "../../utils/utils";

import SinglePicker from "./components/SinglePicker/index";
import MultiPicker from "./components/MultiPicker/index";
import DatePicker1 from "./components/DatePicker/index";
import DateTimePicker from "./components/DateTimePicker";
import InputNumber1 from "./components/InputNumber/index";
import ImagePicker from "./components/ImagePicker/index";
import UserPicker from "./components/UserPicker/index";
import DepartmentPicker from "./components/DepartmentPicker/index";
import SeafileUpload from "./components/SeafileUpload/index";
import CascaderPicker from "./components/CascaderPicker";
import TitleValueList from "./components/TitleValueList";

const { TextArea } = Input;
const validateMessages = {
  required: "必填要素",
};

const ApprovalEditPage = () => {
  const params = useParams();
  const templateName = params.name;
  const itemId = params.id;
  const [templateInfo, setTemplateInfo] = useState({});
  const [form] = Form.useForm();
  const [deptUsers, setDeptUsers] = useState([]);
  const [departmentIds, setDepartmentIds] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [submitButtonDisabled, setSubmitButtonDisabled] = useState(false);
  const [draftButtonDisabled, setDraftButtonDisabled] = useState(false);
  const [flows, setFlows] = useState([]);
  const [isShow, setIsShow] = useState(false);
  const { refreshTab, replaceTab } = useTabLayout();
  const history = useHistory();

  useEffect(() => {
    if (templateName) {
      GetTemplate(templateName, (data) => {
        setTemplateInfo(data);
      });
      GetDepartmentIds(templateName, (data) => {
        setDepartmentIds(data);
      });
    }
  }, [templateName]);

  useEffect(() => {
    GetDeptUsers((data) => {
      setDeptUsers(data);
    });
  }, []);

  useEffect(() => {
    GetDepartments((data) => {
      if (data) {
        const r = data.map((o) => ({
          id: o.profiles.departmentId,
          title: o.profiles.departmentName,
        }));
        if (departmentIds === null) {
          setDepartments(r);
        } else {
          let t = [];
          departmentIds.forEach((o) => {
            const p = r.findIndex((p) => p.id === o);
            if (p !== -1) {
              t.push(r[p]);
            }
          });
          setDepartments(t);
        }
      }
    });
  }, [departmentIds]);

  useEffect(() => {
    if (itemId && itemId !== "0") {
      GetApprovalItem(itemId, (data) => {
        if (data.isUpdate) {
          setSubmitButtonDisabled(false);
        }
        if (data.status) {
          setDraftButtonDisabled(data.status !== 1 ? true : false);
        }
        if (data.content) {
          const formFieldValues = dotKeyToNested(data.content);
          if (formFieldValues) {
            Object.keys(formFieldValues).forEach((o) => {
              if (typeof formFieldValues[o] === "object") {
                formFieldValues[o] = Object.keys(formFieldValues[o]).map(
                  (t) => formFieldValues[o][t]
                );
              }
            });
          }
          form.setFieldsValue(formFieldValues);
        }
      });
    }
  }, [form, itemId]);

  const normImagePath = (e) => {
    let fileList = [...e.fileList];
    fileList &&
      (fileList = fileList.map((file) => {
        if (file.response) {
          file.url =
            typeof file.response.data === "object"
              ? file.response.data.url
              : file.response.data;
        }
        return file;
      }));
    return fileList
      .filter((o) => !!o.status)
      .map((o) => ({ uid: o.uid, url: o.url, status: "done" }));
  };

  const normSeafilePath = (e) => {
    let flag = false;
    let fileList = e.fileList.map((file) => {
      if (file.status !== "done") {
        flag = true;
      }
      if (file.response) {
        file.url =
          typeof file.response.data === "object"
            ? file.response.data.url
            : file.response.data;
        file.fileId =
          typeof file.response.data === "object"
            ? file.response.data.fileId
            : file.response.data;
        file.fileName =
          typeof file.response.data === "object"
            ? file.response.data.fileName
            : file.response.data;
        file.filePath =
          typeof file.response.data === "object"
            ? file.response.data.filePath
            : file.response.data;
        file.repoId =
          typeof file.response.data === "object"
            ? file.response.data.repoId
            : file.response.data;
        file.repoName =
          typeof file.response.data === "object"
            ? file.response.data.repoName
            : file.response.data;
        file.title =
          typeof file.response.data === "object"
            ? file.response.data.title
            : file.response.data;
      }
      return file;
    });
    setSubmitButtonDisabled(flag);
    setDraftButtonDisabled(flag);
    return fileList.map((o) => ({
      uid: o.uid,
      url: o.url,
      fileId: o.fileId,
      fileName: o.fileName,
      filePath: o.filePath,
      repoId: o.repoId,
      repoName: o.repoName,
      title: o.title,
      name: o.title,
      status: o.status,
    }));
  };

  const handleOk = async (submitType) => {
    form.validateFields().then((values) => {
      setSubmitButtonDisabled(true);
      setDraftButtonDisabled(true);
      const formValues = nestedToDotKey(values);
      formValues["oldId"] = itemId;
      if (itemId && itemId !== "0") {
        UpdateApprovalItem(
          templateName,
          itemId,
          formValues,
          submitType,
          (data) => {
            if (data) {
              message.success("保存成功", 1, () => {
                setSubmitButtonDisabled(data.status !== 1 ? true : false);
                setDraftButtonDisabled(data.status !== 1 ? true : false);
                refreshTab("/approvals");
              });
            }
          }
        );
      } else {
        CreateApprovalItem(templateName, formValues, submitType, (data) => {
          if (data) {
            message.success("保存成功");
            history.push(`/approvals/${templateName}/${data.id}/edit`);
            replaceTab({
              oldKey: `/approvals/${templateName}/0/edit`,
              newKey: `/approvals/${templateName}/${data.id}/edit`,
              title: `编辑 ${data.title}`,
            });
            setSubmitButtonDisabled(data.status !== 1 ? true : false);
            setDraftButtonDisabled(data.status !== 1 ? true : false);
            refreshTab("/approvals");
          }
        });
      }
    });
  };

  //
  const handleView = () => {
    form.validateFields().then((values) => {
      const formValues = nestedToDotKey(values);
      PreviewFlow(templateName, formValues, (data) => {
        console.log(data);
        setFlows(data);
        setIsShow(true);
      });
    });
  };

  const handleSubmit = (values) => {
    console.log(values)
  };

  return (
    <PageWrapper title="编辑审批">
      <Form
        form={form}
        onFinish={handleSubmit}
        validateMessages={validateMessages}
      >
        {templateInfo &&
          templateInfo.fields &&
          templateInfo.fields.map((o) => {
            let field;
            switch (o.controlType) {
              case "cascader-picker":
                field = (
                  <Form.Item
                    key={o.name}
                    label={o.title}
                    name={o.name}
                    rules={[{ required: o.required }]}
                    {...formItemLayout}
                  >
                    <CascaderPicker
                      title={o.title}
                      range={o.controlOptions.options || []}
                    />
                  </Form.Item>
                );
                break;
              case "single-picker":
                field = (
                  <Form.Item
                    key={o.name}
                    label={o.title}
                    name={o.name}
                    rules={[{ required: o.required }]}
                    {...formItemLayout}
                  >
                    <SinglePicker
                      title={o.title}
                      range={o.controlOptions.options || []}
                    />
                  </Form.Item>
                );
                break;
              case "multi-picker":
                field = (
                  <Form.Item
                    key={o.name}
                    label={o.title}
                    name={o.name}
                    rules={[{ required: o.required }]}
                    {...formItemLayout}
                  >
                    <MultiPicker
                      title={o.title}
                      range={o.controlOptions.options || []}
                    />
                  </Form.Item>
                );
                break;
              case "date-picker":
                field = (
                  <Form.Item
                    key={o.name}
                    label={o.title}
                    name={o.name}
                    rules={[{ required: o.required }]}
                    {...formItemLayout}
                  >
                    <DatePicker1 />
                  </Form.Item>
                );
                break;
              case "datetime-picker":
                field = (
                  <Form.Item
                    key={o.name}
                    label={o.title}
                    name={o.name}
                    rules={[{ required: o.required }]}
                    {...formItemLayout}
                  >
                    <DateTimePicker />
                  </Form.Item>
                );
                break;
              case "input":
                field = (
                  <Form.Item
                    key={o.name}
                    label={o.title}
                    name={o.name}
                    rules={[{ required: o.required }]}
                    extra={o.controlOptions && o.controlOptions.memo}
                    {...formItemLayout}
                  >
                    <Input disabled={o.controlOptions?.disabled} />
                  </Form.Item>
                );
                break;
              case "input-number":
                field = (
                  <Form.Item
                    key={o.name}
                    label={o.title}
                    name={o.name}
                    rules={[{ required: o.required }]}
                    extra={o.controlOptions && o.controlOptions.memo}
                    {...formItemLayout}
                  >
                    <InputNumber1 />
                  </Form.Item>
                );
                break;
              case "textarea":
                field = (
                  <Form.Item
                    key={o.name}
                    label={o.title}
                    name={o.name}
                    rules={[{ required: o.required }]}
                    extra={o.controlOptions && o.controlOptions.memo}
                    {...formItemLayout}
                  >
                    <TextArea rows={4} />
                  </Form.Item>
                );
                break;
              case "image-picker":
                field = (
                  <Form.Item
                    key={o.name}
                    label={o.title}
                    name={o.name}
                    getValueFromEvent={normImagePath}
                    valuePropName="fileList"
                    rules={[{ required: o.required }]}
                    {...formItemLayout}
                  >
                    <ImagePicker />
                  </Form.Item>
                );
                break;
              case "user":
              case "user-department-picker":
                field = (
                  <Form.Item
                    key={o.name}
                    label={o.title}
                    name={o.name}
                    rules={[{ required: o.required }]}
                    {...formItemLayout}
                  >
                    <UserPicker title={o.title} dataSource={deptUsers} />
                  </Form.Item>
                );
                break;
              case "department":
                field = (
                  <Form.Item
                    key={o.name}
                    label={o.title}
                    name={o.name}
                    rules={[{ required: o.required }]}
                    {...formItemLayout}
                  >
                    <DepartmentPicker name={o.title} range={departments} />
                  </Form.Item>
                );
                break;
              case "seafile-upload":
                field = (
                  <Form.Item
                    key={o.name}
                    label={o.title}
                    name={o.name}
                    getValueFromEvent={normSeafilePath}
                    valuePropName="fileList"
                    rules={[{ required: o.required }]}
                    {...formItemLayout}
                  >
                    <SeafileUpload
                      templateName={templateName}
                      memo={(o.controlOptions && o.controlOptions.memo) || null}
                    />
                  </Form.Item>
                );
                break;
              case "title-value-list":
                field = (
                  <TitleValueList
                    key={o.name}
                    value={form.getFieldValue(o.name)}
                    label={o.title}
                    name={o.name}
                    templateName={templateName}
                    options={o.controlOptions}
                  />
                );
                break;
              default:
                break;
            }
            return field;
          })}

        <Form.Item {...formItemLayoutWithoutLabel}>
          <Button
            type="primary"
            disabled={submitButtonDisabled}
            onClick={() => handleOk("submit")}
          >
            <FontAwesomeIcon icon="check" /> 提交
          </Button>
          <Button
            type="default"
            disabled={draftButtonDisabled}
            onClick={() => handleOk("draft")}
          >
            <FontAwesomeIcon icon="save" /> 草稿
          </Button>
          <Button
            type="dashed"
            disabled={draftButtonDisabled}
            onClick={() => handleView()}
          >
            <FontAwesomeIcon icon="eye" /> 预览流程
          </Button>
        </Form.Item>
      </Form>
      <Card
        style={isShow ? { marginTop: 10 } : { marginTop: 10, display: "none" }}
        title="审批流程"
        extra={
          <Button onClick={() => setIsShow(false)}>
            <FontAwesomeIcon icon="eye-slash" />
            隐藏
          </Button>
        }
      >
        <ApprovalFlow approvalItem={flows || []} />
      </Card>
    </PageWrapper>
  );
};
export default ApprovalEditPage;

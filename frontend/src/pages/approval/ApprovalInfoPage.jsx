import React, { useState, useEffect } from "react";
import {
  Card,
  Descriptions,
  Image,
  Button,
  Popconfirm,
  Tag,
  message,
} from "antd";
import { PageWrapper } from "../../components/PageWrapper";
import { useParams, useHistory } from "react-router-dom";
import { useTabLayout } from "../../hooks/useTabLayout";
import ApprovalLine from "./components/ApprovalLine/index";
import {
  GetApprovalItem,
  GetApprovalUsers,
  ExportPdf,
  CreateCopyItem,
  RecallItem,
  PressItem,
} from "./services/approval";
import { dotKeyToNested } from "../../utils/utils";
import { token } from "../../utils/token";

const ApprovalInfoPage = () => {
  const params = useParams();
  const itemId = params.id;
  const userId = Number(token.getUser().nameid || "0");
  const [content, setContent] = useState({});
  const [fields, setFields] = useState([]);
  const [template, setTemplate] = useState({});
  const [item, setItem] = useState();
  const { refreshSwitches, refreshTab, addTab } = useTabLayout();
  const history = useHistory();
  const refreshSwitch = refreshSwitches[`/approvals/${itemId}/info`];
  const [mentionUsers, setMentionUsers] = useState([]);
  const [canCopyItem, setCanCopyItem] = useState(false);

  useEffect(() => {
    if (itemId) {
      console.log(itemId)
      GetApprovalItem(itemId, (data) => {
        if (data.content) {
          console.log(data)
          const formFieldValues = dotKeyToNested(data.content);
          setItem(data);
          setFields(data.template.fields); //结构
          console.log(formFieldValues)
          setContent(formFieldValues); //数据
          setTemplate(data.template);
        }
        if (data.creatorId) {
          setCanCopyItem(data.creatorId === userId);
        }
      });

      GetApprovalUsers({ itemId }, (data) => {
        if (data) {
          const users =
            data.length > 0 &&
            data.map((u) => {
              return (
                u.profiles &&
                u.profiles.fullName && {
                  name: u.profiles.fullName,
                  id: `${u.id}`,
                }
              );
            });
          setMentionUsers(users);
        }
      });
    }
  }, [itemId, refreshSwitch, userId]);

  //重新提交
  const handleCreateCopy = (id) => {
    CreateCopyItem(id, (res) => {
      if (res.id) {
        addTab({
          key: `/approvals/${res.templateName}/${res.id}/edit`,
          title: `编辑${res.templateTitle}`,
          prev: "/approvals",
          closable: true,
        });
        history.push(`/approvals/${res.templateName}/${res.id}/edit`);
      }
    });
  };

  //撤回
  const handleRecall = (id) => {
    RecallItem(id, (data) => {
      if (data) {
        message.info("操作成功");
        refreshTab("/approvals");
        refreshTab(`/approvals/${itemId}/info`);
      }
    });
  };

  //催办
  const handlePress = (id) => {
    PressItem(id, (data) => {
      if (data) {
        message.info("操作成功");
        refreshTab("/approvals");
      }
    });
  };

  const getStatus = (record) => {
    const text = record.status;
    const color =
      text === "draft"
        ? "gold"
        : text === "approving"
        ? "blue"
        : text === "approved"
        ? "green"
        : text === "rejected"
        ? "red"
        : text === "cancel"
        ? "magenta"
        : "geekblue";
    let info =
      text === "draft"
        ? "草稿"
        : text === "approving"
        ? "待审批"
        : text === "approved"
        ? "通过"
        : text === "rejected"
        ? "拒绝"
        : text === "cancel"
        ? "取消"
        : "需要上传终稿";

    return <Tag color={color}>{info}</Tag>;
  };

  return (
    <PageWrapper
      title="详细信息"
      extras={
        <Button
          type="primary"
          onClick={() => {
            ExportPdf(itemId, (link) => window.open(link, "oa-zysk-pdf"));
          }}
        >
          打印
        </Button>
      }
    >
      <Card
        title="基本信息"
        extra={
          <>
            {item && item.status !== 1 && canCopyItem && (
              <Popconfirm
                title="重新提交创建新的申请，请修改内容后提交，确认提交吗？"
                onConfirm={() => {
                  handleCreateCopy(itemId);
                }}
                okText="是"
                cancelText="否"
              >
                <Button type="primary">重新提交</Button>
              </Popconfirm>
            )}
            {item &&
              item.status === 2 &&
              !item.isUpdate &&
              item.creatorId === userId && (
                <Popconfirm
                  title="确定撤回吗？"
                  onConfirm={() => {
                    handleRecall(itemId);
                  }}
                  okText="是"
                  cancelText="否"
                >
                  <Button type="primary">撤回</Button>
                </Popconfirm>
              )}
            {item && item.status === 2 && item.creatorId === userId && (
              <Popconfirm
                title="确定催办吗？"
                onConfirm={() => {
                  handlePress(itemId);
                }}
                okText="是"
                cancelText="否"
              >
                <Button type="primary">催办</Button>
              </Popconfirm>
            )}
          </>
        }
      >
        <Descriptions column={1}>
          <Descriptions.Item key="code" label="编号">
            {(item && item.code) || ""}
          </Descriptions.Item>
          {fields &&
            fields.map((o) => {
              let field;
              switch (o.controlType) {
                case "single-picker":
                case "date-picker":
                case "datetime-picker":
                case "input":
                case "input-number":
                case "textarea":
                  let text = content && content[o.name] ? content[o.name] : "";
                  if (o.controlOptions && o.controlOptions.currency) {
                    text = isNaN(parseFloat(text))
                      ? text
                      : parseFloat(text).toFixed(2);
                  }
                  if (o.controlOptions && o.controlOptions.suffix) {
                    text = text ? text + o.controlOptions.suffix : "";
                  }
                  field = (
                    <Descriptions.Item key={o.title} label={o.title}>
                      {text || "无"}
                    </Descriptions.Item>
                  );
                  break;
                case "multi-picker":
                  field = (
                    <Descriptions.Item key={o.title} label={o.title}>
                      {content &&
                        content[o.name] &&
                        Object.keys(content[o.name]).map((k) => (
                          <div
                            style={{ marginRight: 10 }}
                            key={content[o.name][k]}
                          >
                            {content[o.name][k]}
                          </div>
                        ))}
                    </Descriptions.Item>
                  );
                  break;
                case "image-picker":
                  field = (
                    <Descriptions.Item key={o.title} label={o.title}>
                      {content &&
                        content[o.name] &&
                        Object.keys(content[o.name]).map((k) => (
                          <div
                            style={{ marginRight: 10 }}
                            key={content[o.name][k].url}
                          >
                            <Image
                              width={100}
                              height={100}
                              src={content[o.name][k].url}
                            />
                          </div>
                        ))}
                    </Descriptions.Item>
                  );
                  break;
                case "user":
                case "user-department-picker":
                  field = (
                    <Descriptions.Item key={o.title} label={o.title}>
                      {content &&
                        content[o.name] &&
                        Object.keys(content[o.name]).map((k) => (
                          <div
                            style={{ marginRight: 10 }}
                            key={content[o.name][k].name}
                          >
                            {content[o.name][k].name}
                          </div>
                        ))}
                    </Descriptions.Item>
                  );
                  break;
                case "department":
                  field = (
                    <Descriptions.Item key={o.title} label={o.title}>
                      {content &&
                        content[o.name] &&
                        Object.keys(content[o.name]).map((k) => (
                          <div key={k} style={{ marginRight: 10 }}>
                            {content[o.name][k].title}
                          </div>
                        ))}
                    </Descriptions.Item>
                  );
                  break;
                case "seafile-upload":
                  field = (
                    <Descriptions.Item
                      key={o.title}
                      label={o.title}
                      className="auto-wrap"
                    >
                      {(content &&
                        content[o.name] &&
                        Object.keys(content[o.name]).map((k) => (
                          <>
                            <div
                              style={{ marginRight: 10 }}
                              key={content[o.name][k].filePath}
                            >
                              <a
                                target="_blank"
                                rel="noopener noreferrer"
                                href={content[o.name][k].url}
                              >
                                {content[o.name][k].title}
                              </a>
                            </div>
                            <br />
                          </>
                        ))) ||
                        "无"}
                    </Descriptions.Item>
                  );
                  break;
                case "title-value-list":
                  field = (
                    <Descriptions.Item key={o.title} label={o.title}>
                      <table className="ant-table">
                        <tbody>
                          {content &&
                            content[o.name] &&
                            Object.keys(content[o.name]).map((k, i) => (
                              <tr key={k}>
                                <td>{i + 1}. </td>
                                <td>{content[o.name][k].fapiao}</td>
                                <td>{content[o.name][k].title}</td>
                                <td>
                                  {content[o.name][k].value}
                                  {o.controlOptions.suffix}
                                </td>
                              </tr>
                            ))}
                        </tbody>
                      </table>
                    </Descriptions.Item>
                  );
                  break;
                default:
                  break;
              }
              return field;
            })}
          {item && (
            <Descriptions.Item key="status" label="状态">
              {getStatus(item)}
            </Descriptions.Item>
          )}
          {item && item.template && item.template.name === "overtime" && (
            <Descriptions.Item key="finishDate" label="实际结束日期">
              {(content && content["finishDate"]) || "无"}
            </Descriptions.Item>
          )}
          {item && item.finalFiles && item.finalFiles.length > 0 && (
            <Descriptions.Item key="finalFiles" label="终稿">
              {item.finalFiles.map((o) => (
                <div style={{ marginRight: 10 }} key={o.url}>
                  <a target="_blank" rel="noopener noreferrer" href={o.url}>
                    {o.title}
                  </a>
                </div>
              ))}
            </Descriptions.Item>
          )}
          {item && item.verifiedFiles && item.verifiedFiles.length > 0 && (
            <Descriptions.Item key="verifiedFiles" label="发布稿">
              {item.verifiedFiles.map((o) => (
                <div style={{ marginRight: 10 }} key={o.url}>
                  <a target="_blank" rel="noopener noreferrer" href={o.url}>
                    {o.title}
                  </a>
                </div>
              ))}
            </Descriptions.Item>
          )}
        </Descriptions>
      </Card>
      <Card style={{ marginTop: 10 }} title="审批流程">
        <ApprovalLine
          approvalItem={item || { id: itemId, nodes: [] }}
          mentionUsers={mentionUsers}
          template={template}
          onSave={() => {
            refreshTab(`/approvals/${itemId}/info`);
          }}
        />
      </Card>
    </PageWrapper>
  );
};
export default ApprovalInfoPage;

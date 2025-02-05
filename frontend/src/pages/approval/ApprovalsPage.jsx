import React, { useState, useEffect, useCallback, Fragment } from "react";
import { PageWrapper } from "../../components/PageWrapper";
import {
  Menu,
  Select,
  Dropdown,
  Button,
  Popconfirm,
  Tag,
  Table,
  message,
  Input,
  Upload,
  Modal,
  Row,
  DatePicker,
} from "antd";
import { useTabLayout } from "../../hooks/useTabLayout";
import { useHistory } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  GetTemplateNames,
  GetApprovalItems,
  DeleteApprovalItem,
  GetTemplateGroup,
  DeleteFinalFile,
  UpdateBackTime,
} from "./services/approval";
import dayjs from "dayjs";
import { token } from "../../utils/token";

let uploadIndex = 0;
//#region 上传图片
const uploadButton = (
  <div>
    <Button icon={<FontAwesomeIcon icon="upload" />}>上传终稿</Button>
  </div>
);
//#endregion

const generatorUploadItem = (url, title) => ({
  uid: (--uploadIndex).toString(),
  name: title,
  status: "done",
  url,
});

const ApprovalsPage = () => {
  const [groups, setGroups] = useState({});
  const [templateGroup, setTemplateGroup] = useState([]);
  const [options, setOptions] = useState([]);
  const { addTab, refreshSwitches } = useTabLayout();
  const history = useHistory();
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({ pageSize: 20 });
  const [currentPage, setCurrentPage] = useState(1);
  const [list, setList] = useState([]);
  const [filters, setFilters] = useState({ templateName: "" });
  const refreshSwitch = refreshSwitches["/approvals"];
  const [showModal, setShowModal] = useState(false);
  const [currentItem, setCurrentItem] = useState({});
  const [finalFiles, setFinalFiles] = useState([]);
  const [showBackTime, setShowBackTime] = useState(false);
  const [backTime, setBackTime] = useState(undefined);

  const columns = [
    {
      title: "标题",
      dataIndex: "title",
    },
    {
      title: "模板类型",
      dataIndex: "templateGroup",
      render: (v) => {
        return groups && groups[v];
      },
    },
    {
      title: "审批类型",
      dataIndex: "templateTitle",
    },
    {
      title: "摘要",
      dataIndex: "summary",
      width: "20%",
    },
    {
      title: "创建日期",
      dataIndex: "createdTime",
      width: "10%",
      render: (text) => {
        return dayjs(text).format("YYYY-MM-DD");
      },
    },
    {
      title: "状态",
      dataIndex: "status",
      width: "10%",
      render: (text, record) => {
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
      },
    },
    {
      title: "操作",
      width: 240,
      render: (text, record) => (
        <Fragment>
          {record.isUpdate && (
            <Button
              type="link"
              onClick={() => {
                addTab({
                  key: `/approvals/${record.templateName}/${record.id}/edit`,
                  title: `编辑${record.templateTitle}`,
                  prev: "/approvals",
                  closable: true,
                });
                history.push(
                  `/approvals/${record.templateName}/${record.id}/edit`
                );
              }}
            >
              编辑
            </Button>
          )}
          {record.status === 1 && (
            <>
              <Popconfirm
                title="确认删除?"
                onConfirm={() => {
                  DeleteApprovalItem(record.id, (data) => {
                    if (data) {
                      message.success("操作成功", 1, () => {
                        refreshTable();
                      });
                    }
                  });
                }}
                okText="是"
                cancelText="否"
              >
                <Button type="link">删除</Button>
              </Popconfirm>
            </>
          )}
          <Button
            type="link"
            onClick={() => {
              addTab({
                key: `/approvals/${record.id}/info`,
                title: `查看${record.templateTitle}`,
                prev: "/approvals",
                closable: true,
              });
              history.push(`/approvals/${record.id}/info`);
            }}
          >
            详细
          </Button>
          {record.isFinal && (
            <Button
              type="link"
              onClick={() => {
                if (record.finalFiles && record.finalFiles.length > 0) {
                  setFinalFiles(
                    record.finalFiles.map((file) =>
                      generatorUploadItem(file.url, file.title)
                    )
                  );
                } else {
                  setFinalFiles([]);
                }
                setCurrentItem(record);
                setShowModal(true);
              }}
            >
              终稿
            </Button>
          )}
          {record.templateName === "out" &&
            record.status === 3 &&
            Object.keys(record.content).indexOf("confirmDate") > -1 &&
            record.content.confirmDate === null && (
              <Button
                type="link"
                onClick={() => {
                  setCurrentItem(record);
                  setShowBackTime(true);
                }}
              >
                补充返回时间
              </Button>
            )}
        </Fragment>
      ),
    },
  ];

  const handleUploadEvent = (e, itemId) => {
    switch (e.file.status) {
      case "done":
        message.success("上传成功!");
        refreshTable();
        break;
      case "removed":
        DeleteFinalFile(itemId, e.file.name, (data) => {
          message.success("删除成功!");
          refreshTable();
        });
        break;
      default:
        break;
    }
  };
  const getGroups = (data) => {
    let t = [];
    const parent = [...new Set(data.map((o) => o.groupTitle))] || [];
    parent.forEach((o) => {
      const children = data.filter((f) => f.groupTitle === o);
      t.push({ title: o, children });
    });
    return t;
  };

  useEffect(() => {
    GetTemplateNames((data) => {
      setTemplateGroup(getGroups(data));
      setOptions(data.map((o) => ({ label: o.title, value: o.name })));
    });
    GetTemplateGroup((data) => {
      setGroups(data);
    });
  }, []);

  const refreshTable = useCallback(() => {
    setLoading(true);
    GetApprovalItems({ filters, page: currentPage, size: 20 }, (result) => {
      setList(result.data);
      setPagination({
        pageSize: 20,
        total: result.total,
      });
      setLoading(false);
    });
  }, [currentPage, filters]);

  useEffect(() => {
    refreshTable();
  }, [currentPage, refreshTable, refreshSwitch]);

  const handleTableChange = (pagination) => {
    setCurrentPage(pagination.current);
  };

  const onSelectChange = (v) => {
    setBackTime(v ? dayjs(v).format("YYYY-MM-DD HH:mm") : null);
  };

  return (
    <>
      {showModal && (
        <Modal
          title={`${currentItem && currentItem.title} 终稿文件`}
          width={800}
          maskClosable={false}
          visible={true}
          onCancel={() => {
            setShowModal(false);
          }}
          footer={null}
        >
          <Row>
            <Upload
              name="file"
              multiple={false}
              listType="text"
              // className='avatar-uploader'
              showUploadList={true}
              onChange={(e) => {
                handleUploadEvent(e, currentItem.id);
              }}
              action={`/api/approval/final/${currentItem.id}/upload`}
              defaultFileList={finalFiles || []}
              headers={{
                Authorization: `Bearer ${token.get()}`,
              }}
            >
              {uploadButton}
            </Upload>
          </Row>
          {/* <Row>
          <Table dataSource={finalFiles || []} columns={fileColumns} rowKey='url' pagination={false} />
        </Row> */}
        </Modal>
      )}
      {showBackTime && (
        <Modal
          title="补充实际返回时间"
          width={400}
          maskClosable={false}
          visible={true}
          onOk={() => {
            if (backTime) {
              UpdateBackTime(currentItem.id, { backTime }, (data) => {
                if (data) {
                  refreshTable();
                  setShowBackTime(false);
                  message.success("操作成功");
                }
              });
            } else {
              message.error("请输入正确时间");
            }
          }}
          onCancel={() => {
            setShowBackTime(false);
          }}
        >
          <Row>
            <DatePicker
              defaultValue={null}
              showTime={{ hideDisabledOptions: true, format: "HH:mm" }}
              onChange={onSelectChange}
            />
          </Row>
        </Modal>
      )}
      <PageWrapper
        title="审批管理"
        majorAction={
          <Dropdown
            overlay={
              <Menu>
                {templateGroup &&
                  templateGroup.map((o) => (
                    <Menu.ItemGroup key={o.title} title={o.title}>
                      {o.children &&
                        o.children.map((t) => (
                          <Menu.Item
                            key={t.id}
                            onClick={() => {
                              addTab({
                                key: `/approvals/${t.name}/0/edit`,
                                title: `编辑 ${t.title}`,
                                prev: "/approvals",
                                closable: true,
                              });
                              history.push(`/approvals/${t.name}/0/edit`);
                            }}
                          >
                            {t.title}
                          </Menu.Item>
                        ))}
                    </Menu.ItemGroup>
                  ))}
              </Menu>
            }
          >
            <Button>新建</Button>
          </Dropdown>
        }
        extras={
          <>
            <Select
              options={[{ label: "全部类型", value: "" }, ...options]}
              onChange={(v) => {
                setFilters((f) => ({ ...f, templateName: v }));
                setCurrentPage(1);
              }}
              style={{ width: 160 }}
              value={filters.templateName}
            />
            <Input
              style={{ width: 150 }}
              placeholder="检索关键字"
              onPressEnter={(e) => {
                const key = e.target.value;
                setFilters((f) => ({ ...f, query: key }));
                setCurrentPage(1);
              }}
            />
          </>
        }
      >
        <Table
          loading={loading}
          dataSource={list || []}
          columns={columns}
          rowKey="id"
          pagination={{ ...pagination, current: currentPage }}
          onChange={handleTableChange}
        />
      </PageWrapper>
    </>
  );
};
export default ApprovalsPage;

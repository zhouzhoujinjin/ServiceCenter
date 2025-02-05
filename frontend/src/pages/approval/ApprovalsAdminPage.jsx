import React, { useState, useEffect, useCallback, Fragment } from "react";
import { PageWrapper } from "../../components/PageWrapper";
import { Select, Button, Tag, Table, Input, DatePicker, message } from "antd";
import { useTabLayout } from "../../hooks/useTabLayout";
import { useHistory } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  GetTemplateAll,
  GetApprovalItemsAdmin,
  GetTemplateGroup,
  ApprovalExport,
} from "./services/approval";
import dayjs from "dayjs";
import { faDownload } from "@fortawesome/pro-light-svg-icons";
const approvalStatus = [
  { label: "草稿", value: "Draft" },
  { label: "待审批", value: "Approving" },
  { label: "通过", value: "Approved" },
  { label: "拒绝", value: "Rejected" },
  { label: "需要上传终稿", value: "Upload" },
];

const { RangePicker } = DatePicker;
const ApprovalsAdminPage = () => {
  const [groups, setGroups] = useState({});
  const [options, setOptions] = useState([]);
  const { addTab } = useTabLayout();
  const history = useHistory();
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({ pageSize: 10 });
  const [currentPage, setCurrentPage] = useState(1);
  const [list, setList] = useState([]);
  const [filters, setFilters] = useState({ templateName: "", status: "" });

  const columns = [
    {
      title: "标题",
      dataIndex: "title",
    },
    {
      title: "模板类型",
      dataIndex: "templateGroup",
      width: "15%",
      render: (v) => {
        return groups && groups[v];
      },
    },
    {
      title: "审批类型",
      dataIndex: "templateTitle",
      width: "15%",
    },
    {
      title: "创建人",
      dataIndex: ["creator", "profiles", "fullName"],
      width: 80,
    },
    {
      title: "创建日期",
      dataIndex: "createdTime",
      width: 120,
      render: (text) => {
        return dayjs(text).format("YYYY-MM-DD");
      },
    },
    {
      title: "状态",
      dataIndex: "status",
      width: 100,
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
      render: (text, record) => (
        <Fragment>
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
        </Fragment>
      ),
      width: 150,
    },
  ];

  useEffect(() => {
    GetTemplateAll((data) => {
      setOptions(Object.keys(data).map((k) => ({ label: data[k], value: k })));
    });
    GetTemplateGroup((data) => {
      setGroups(data);
    });
  }, []);

  const refreshTable = useCallback(() => {
    setLoading(true);
    GetApprovalItemsAdmin(
      { filters, page: currentPage, size: 10 },
      (result) => {
        setList(result.data);
        setPagination({
          pageSize: 10,
          total: result.total,
        });
        setLoading(false);
      }
    );
  }, [currentPage, filters]);

  useEffect(() => {
    refreshTable();
  }, [currentPage, refreshTable]);

  const handleTableChange = (pagination) => {
    setCurrentPage(pagination.current);
  };

  const handleExport = () => {
    if (filters.startDate !== "" && filters.endDate !== "") {
      try {
        message.success("正在处理 请耐心等候");
        ApprovalExport(filters, (data) => {
          if (data == null) {
            message.error("未查询到数据");
          } else {
            window.open(data, "");
          }
        });
      } catch (error) {
        message.error("导出错误");
      }
    } else {
      message.error("时间 为必填项");
    }
  };

  return (
    <PageWrapper
      title="全部申请"
      majorAction={null}
      extras={
        <>
          <RangePicker
            showTime
            onChange={(e) => {
              if (e != null) {
                setFilters((f) => ({
                  ...f,
                  startDate: dayjs(e[0]).format("YYYY-MM-DD HH:mm:ss"),
                  endDate: dayjs(e[1]).format("YYYY-MM-DD HH:mm:ss"),
                }));
              }
            }}
          />
          <Select
            options={[{ label: "审批状态", value: "" }, ...approvalStatus]}
            onChange={(v) => {
              setFilters((f) => ({ ...f, status: v }));
              setCurrentPage(1);
            }}
            style={{ width: 160 }}
            value={filters.status}
          />
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
          <Button type="default" onClick={() => handleExport()}>
            <FontAwesomeIcon icon={faDownload} /> 导出
          </Button>
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
  );
};
export default ApprovalsAdminPage;

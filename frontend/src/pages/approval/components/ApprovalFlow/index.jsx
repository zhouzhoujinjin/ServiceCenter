import React, { useState, useEffect } from "react";
import { Timeline, Row, Col } from "antd";
import { token } from "../../../../utils/token";
import { uuid } from "../../../../utils/utils";
import TimelineItem from "./TimelineItem";
import ApprovalNode from "./ApprovalNode";

const NodeType = {
  unknown: {
    title: "未知",
    name: "unknown",
  },
  start: {
    title: "发起",
    name: "start",
    icon: "alert-circle",
  },
  approval: {
    title: "审批",
    name: "approval",
    icon: "user-circle",
  },
  cc: {
    title: "抄送",
    name: "cc",
    icon: "",
  },
  and: {
    title: "会签",
    name: "and",
  },
  or: {
    title: "或签",
    name: "or",
  },
  sub: {
    title: "子",
    name: "sub",
  },
};

const ActionType = {
  DeleteApprovers: -5,
  ChosenCC: -4,
  ChooseCC: -3,
  ChosenApprover: -2,
  ChooseApprover: -1,
  Created: 1,
  Approving: 2,
  Approved: 3,
  Rejected: 4,
  Comment: 5,
};

const ActionTitle = {
  [ActionType.Approving]: "待审批",
  [ActionType.Approved]: "已通过",
  [ActionType.Rejected]: "已拒绝",
};

const getActionString = (actionType, nodeType) => {
  if (nodeType === "cc" && actionType > 0) {
    return "/cc.png";
  }
  switch (actionType) {
    case ActionType.Approved:
      return "/approved.png";
    case ActionType.Rejected:
      return "/rejected.png";
    case ActionType.Approving:
      return "/pending.png";
    default:
      return "";
  }
};

const ApprovalFlow = ({ approvalItem }) => {
  const [originalList, setOrginalList] = useState([]);
  const [list, setList] = useState([]);
  const userId = Number(token.getUser().nameid || "0");

  useEffect(() => {
    setOrginalList(approvalItem);
  }, [approvalItem]);

  useEffect(() => {
    if (originalList.length === 0) {
      return;
    }
    const l = originalList.map((x) => {
      const children = x.children
        ? x.children.map((y) => (
            <ApprovalNode
              key={uuid()}
              badgeIcon={getActionString(y.actionType, y.nodeType)}
              title={y.user && y.user.profiles && y.user.profiles.fullName}
            />
          ))
        : [
            <ApprovalNode
              key={uuid()}
              data={x}
              badgeIcon={getActionString(x.actionType, x.nodeType)}
              title={x.user && x.user.profiles && x.user.profiles.fullName}
            />,
          ];

      const node = {
        id: x.id,
        title: `${NodeType[x.nodeType].title} ${
          ActionTitle[x.actionType] ? "[" + ActionTitle[x.actionType] + "]" : ""
        }`,
        nodeType: x.nodeType,
        color:
          x.actionType === 1 ? "gray" : x.actionType === 2 ? "red" : "green",
        actionType: x.actionType,
        content: children,
        comments: x.comments,
        children: x.children,
        user: x.user,
      };

      return node;
    });

    setList(l);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [originalList, userId]);

  return (
    <>
      <Row>
        <Col span={8}>
          <Timeline>
            {list &&
              list.map((o) => (
                <TimelineItem key={uuid()} extras={o.extras} title={o.title}>
                  {o.content}
                </TimelineItem>
              ))}
          </Timeline>
        </Col>
      </Row>
    </>
  );
};

export default ApprovalFlow;

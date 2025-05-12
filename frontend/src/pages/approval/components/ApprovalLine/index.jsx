import React, { useState, useEffect } from "react";
import {
  Timeline,
  Comment,
  Row,
  Col,
  List,
  Divider,
  Button,
  Modal,
  message,
  Popconfirm,
} from "antd";
import {
  UpdateApproval,
  GetDeptUsers,
  UpdateSelfApproval,
  GetTransUsers,
  TransUser,
  IsUpdate,
  IsLastApproval,
} from "../../services/approval";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import UserPicker from "../UserPicker/index";
import UserSinglePicker from "../UserSinglePicker/index";
import dayjs from "dayjs";
import { token } from "../../../../utils/token";
import { uuid } from "../../../../utils/utils";
import TimelineItem from "./TimelineItem";
import ApprovalNode from "./ApprovalNode";
// import Editor from "../../../../components/Editor";
import { faPenAlt } from "@fortawesome/pro-light-svg-icons";

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
  Created: 'created',
  Approving: 'pending',
  Approved: 'approved',
  Rejected: 'rejected',
  Comment: 'comment',
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

const getLastUsers = (list) => {
  const len = list.length;
  if (len === 0) {
    return [];
  }
  if (list[len - 1].children) {
    return list[len - 1].children.map((x) => x.user);
  } else {
    return [list[len - 1].user];
  }
};

const getAllComments = (nodes) => {
  let arr = [];
  for (let index = 0; index < nodes.length; index++) {
    const element = nodes[index];
    if (element.comments && element.comments.length > 0) {
      element.comments.forEach((o) => arr.push(o));
    }
  }
  arr.sort((a, b) => {
    return Date.parse(a.createdTime) - Date.parse(b.createdTime);
  });
  return arr;
};

const ApprovalLine = ({ approvalItem, mentionUsers, onSave }) => {
  const [originalList, setOrginalList] = useState([]);
  const [list, setList] = useState([]);
  const [currentNode, setCurrentNode] = useState(); //选中的节点
  const [loading, setLoading] = useState(false);

  const userId = Number(token.getUser().nameid || "0");

  const [commentAll, setCommentAll] = useState([]); //当前申请所有意见
  const [comments, setComments] = useState([]); //点击节点的所有意见
  const [comment, setComment] = useState(""); //记录输入的意见
  const [showUserChooser, setShowUserChooser] = useState(false);
  const [showCommentLayout, setShowCommentLayout] = useState(false);
  const [showCommentBtn, setShowCommentBtn] = useState(false);
  const [commentLayoutOffset, setCommentLayoutOffset] = useState();
  const [deptUsers, setDeptUsers] = useState([]); //所有人员
  const [addUserType, setAddUserType] = useState(""); //添加人员类型  approver : 审批人  cc : 抄送人
  const [chooseUsers, setChooseUsers] = useState([]);

  const [showTransUserChooser, setShowTransUserChooser] = useState(false);
  const [zbUsers, setZbUsers] = useState([]); //转办可以选择的人员，排除到流程中已经存在的人员
  const [chooseUser, setChooseUser] = useState(1); //选择的转办人，只支持单个选择
  const [isLastApproval, setIsLastApproval] = useState(false);

  useEffect(() => {
    setOrginalList(approvalItem.nodes);
    setCommentAll(getAllComments(approvalItem.nodes));
  }, [approvalItem.nodes]);

  useEffect(() => {
    console.log('inDept')
    GetDeptUsers((data) => {
      
      setDeptUsers(data);
    });
    GetTransUsers(approvalItem.id, (data) => {
      setZbUsers(data);
    });
    IsLastApproval(approvalItem.id, (data) => {
      setIsLastApproval(data);
    });
  }, []);

  useEffect(() => {
    if (originalList.length === 0) {
      return;
    }
    const l = originalList.map((x) => {
      const children = x.children
        ? x.children.map((y) => (
            <ApprovalNode
              key={y.id}
              badgeIcon={getActionString(y.actionType, y.nodeType)}
              avatar={y.user.profiles.avatar || '/default-user.png'}
              title={y.user.profiles.fullName}
            />
          ))
        : [
            <ApprovalNode
              key={x.id}
              data={x}
              badgeIcon={getActionString(x.actionType, x.nodeType)}
              avatar={x.user?.profiles.avatar || '/default-user.png'}
              title={x.user?.profiles.fullName}
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
      console.log(x.actionType, x.nodeType)
      if (x.actionType === ActionType.ChosenApprover) {
        node.extras = (
          <Button type="link" onClick={() => handleRemoveNode(node)}>
            <FontAwesomeIcon icon={["fal", "trash"]} />
            删除审批
          </Button>
        );
      } else if (x.actionType === ActionType.ChosenCC) {
        node.extras = (
          <Button type="link" onClick={() => handleRemoveNode(node)}>
            <FontAwesomeIcon icon={["fal", "trash"]} />
            删除抄送
          </Button>
        );
      } else if (x.actionType === ActionType.Approving) {
        node.extras = (
          <>
            {x.nodeType !== 'start' &&
              x.nodeType !== 'cc' &&
              x.actionType > ActionType.Created && (
                <Button
                  type="link"
                  onClick={() => {
                    //选择节点
                    setCurrentNode(x);
                    //获取意见
                    setComments(x.comments || []);
                    //是否能发表
                    if (x.actionType === ActionType.Approving) {
                      setShowCommentBtn(true);
                    } else {
                      setShowCommentBtn(false);
                    }
                    setShowCommentLayout(true);
                  }}
                >
                  <FontAwesomeIcon icon={faPenAlt} /> 意见
                </Button>
              )}
          </>
        );
      } else if (
        x.actionType === ActionType.Approved ||
        x.actionType === ActionType.Rejected
      ) {
        node.extras = (
          <>
            {x.nodeType !== 'start' &&
              x.nodeType !== 'cc' &&
              x.actionType > ActionType.Created && (
                <Button
                  type="link"
                  onClick={(e) => {
                    //选择节点
                    setCurrentNode(x);
                    //获取意见
                    setComments(x.comments || []);
                    //是否能发表
                    if (x.actionType === ActionType.Approving) {
                      setShowCommentBtn(true);
                    } else {
                      setShowCommentBtn(false);
                    }
                    setShowCommentLayout(true);
                    var offset = document
                      .getElementById("commentsLayers")
                      .getBoundingClientRect().top;
                    setCommentLayoutOffset(e.clientY - offset);
                  }}
                >
                  <FontAwesomeIcon icon="comment" /> 意见
                </Button>
              )}
          </>
        );
      }

      return node;
    });

    const notCCList = l.filter((x) => x.nodeType !== 'cc');
    const lastNodeUsers = getLastUsers(notCCList);
    const len = notCCList.length;
    const saauBtn =
      (notCCList[len - 1].nodeType === 1 &&
        lastNodeUsers.find((x) => x.userId === userId || x.id === userId)) ||
      (notCCList[len - 1].actionType === ActionType.Approving &&
        !!lastNodeUsers.find((x) => x.userId === userId || x.id === userId)) ||
      notCCList[len - 1].actionType === ActionType.ChosenApprover ||
      notCCList[len - 1].actionType === ActionType.ChosenCC;

    if (
      approvalItem &&
      approvalItem.template &&
      approvalItem.template.isCustomFlow &&
      saauBtn
    ) {
      l.push({
        id: uuid(),
        title: "添加",
        color: "gray",
        actionType: ActionType.ChooseApprover,
        extras: (
          <Popconfirm
            title="确认保存流程吗?"
            onConfirm={() => handleSaveApproval()}
            okText="是"
            cancelText="否"
          >
            <Button type="link">
              <FontAwesomeIcon icon="save" /> 保存流程
            </Button>
          </Popconfirm>
        ),
        content: [
          <ApprovalNode
            key={0}
            avatar="/add-approver.png"
            title="添加审批人"
            className="node-action"
            onClick={() => {
              setAddUserType("approver");
              setChooseUsers([]);
              setShowUserChooser(true);
            }}
          />,
          <ApprovalNode
            key={1}
            avatar="/add-cc.png"
            title="添加抄送人"
            className="node-action"
            onClick={() => {
              setAddUserType("cc");
              setChooseUsers([]);
              setShowUserChooser(true);
            }}
          />,
        ],
      });
    }

    setList(l);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [originalList, userId]);

  const handleRemoveNode = (node) => {
    const index = originalList.findIndex((x) => x.id === node.id);
    originalList.splice(index, 1);
    setOrginalList([...originalList]);
    setCurrentNode(undefined);
  };

  const checkStatus = (entry) => {
    const isUpdate = approvalItem.isUpdate || false; //退改状态
    console.log(entry)
    if (entry.actionType === ActionType.Approving) {
      if (entry.children && entry.children.length) {
        const userNode = entry.children.find(
          (x) => x.userId === Number(userId)
        );
        return userNode && userNode.actionType === ActionType.Approving && !isUpdate;
      } else {
        //todo 退改不能操作
        console.log(entry.user.id, Number(userId), !isUpdate)
        return entry.user.id === Number(userId) && !isUpdate;
      }
    }
    return false;
  };

  //添加审批人
  const handleApprovalUsers = () => {
    console.log(chooseUsers);
    if (chooseUsers && chooseUsers.length > 0) {
      const tmpList = [...originalList];
      let newNode;
      if (addUserType === "approver") {
        //审批人
        if (chooseUsers.length === 1) {
          newNode = {
            id: uuid(),
            comments: [],
            actionType: ActionType.ChosenApprover,
            nodeType: 'approval',
            userId: chooseUsers[0].id,
            user: {
              id: chooseUsers[0].id,
              profiles: {
                fullName: chooseUsers[0].name,
                avatar: chooseUsers[0].avatar,
              },
            },
          };
        } else {
          newNode = {
            id: uuid(),
            comments: [],
            actionType: ActionType.ChosenApprover,
            nodeType: 'and',
            children: chooseUsers.map((x) => ({
              id: uuid(),
              comments: [],
              actionType: ActionType.Created,
              nodeType: 6,
              userId: x.id,
              user: {
                id: x.id,
                profiles: { fullName: x.name, avatar: x.avatar },
              },
            })),
          };
        }
        tmpList.push(newNode);
      } else {
        //抄送人
        chooseUsers.forEach((x) => {
          tmpList.push({
            id: uuid(),
            actionType: ActionType.ChosenCC,
            nodeType: 'cc',
            user: {
              id: x.id,
              profiles: { fullName: x.name, avatar: x.avatar },
            },
            userId: x.id,
          });
        });
      }
      setOrginalList(tmpList);
    }

    setShowUserChooser(false);
  };

  //保存流程
  const handleSaveApproval = () => {
    const newNodes = originalList.filter((o) => o.actionType < 0);
    if (newNodes && newNodes.length > 0) {
      UpdateSelfApproval(
        { itemId: approvalItem.id, nodes: newNodes },
        (data) => {
          if (data) {
            message.success("操作成功");
            onSave && onSave();
          }
        }
      );
    }
  };

  //意见审批
  const handleApproval = (actionType) => {
    if (currentNode) {
      const nodeId = currentNode.id;
      let content = "";
      const text = comment.replace(/(^\s*)|(\s*$)/g, "");

      if (text === "<p><br></p>" && actionType === ActionType.Approved) {
        content = "同意";
      } else if (text === "<p><br></p>" && actionType === ActionType.Rejected) {
        content = "拒绝";
      } else {
        content = text;
      }

      const approval = {
        comment: content,
        actionType: actionType,
      };
      setLoading(true);
      UpdateApproval({ itemId: approvalItem.id, nodeId, approval }, (data) => {
        if (data.id && data.user) {
          if (data.comments) {
            let d = data.comments.map((o) => ({
              content: o.content,
              createdTime: dayjs(o.createdTime).format("YYYY-MM-DD HH:mm:ss"),
              userAvatar: o.userAvatar,
              userFullName: o.userFullName,
            }));
            //将新增意见补充到现有节点
            const index = originalList.findIndex(
              (x) => x.id === currentNode.id
            );
            const current = originalList[index];
            const children = current.children;
            if (children) {
              const nodeIndex = children.findIndex(
                (x) => x.userId === Number(userId)
              );
              if (nodeIndex >= 0) {
                children[nodeIndex] = {
                  ...children[nodeIndex],
                  actionType: actionType,
                };
                current.children = [...children];
              }
            }
            current.comments = [...d];
            originalList[index] = current;
            setOrginalList([...originalList]);
            //
            setComments(d);
            setComment("");
          }
          setShowCommentLayout(false);
        }
        setLoading(false);
        message.success("操作成功", 2, () => {
          onSave && onSave();
        });
      });
    }
  };

  //转办
  const handleTrans = () => {
    if (currentNode) {
      const text = comment.trim().replace(/(^\s*)|(\s*$)/g, "");

      if (text === "<p><br></p>") {
        message.error("意见必填");
        return;
      }
      setChooseUser([]);
      setShowTransUserChooser(true);
    }
  };

  const handleTransUser = () => {
    if (currentNode) {
      const nodeId = currentNode.id;
      const userId = chooseUser.id;
      const trans = {
        userId: userId,
        comment: comment,
      };
      TransUser(nodeId, trans, (data) => {
        if (data) {
          message.success("操作成功");
          setShowTransUserChooser(false);
          onSave && onSave();
        }
      });
    }
  };

  //退改
  const handleUpdate = () => {
    if (currentNode) {
      const text = comment.trim().replace(/(^\s*)|(\s*$)/g, "");

      if (text === "<p><br></p>") {
        message.error("意见必填");
        return;
      }

      const approval = {
        comment: text,
      };
      IsUpdate(approvalItem.id, currentNode.id, approval, (data) => {
        if (data) {
          message.success("操作成功");
          onSave && onSave();
        }
      });
    }
  };

  return (
    <>
      <Row>
        <Col span={4}>
          <Timeline>
            {list &&
              list.map((o) => (
                <TimelineItem key={uuid()} extras={o.extras} title={o.title}>
                  {o.content}
                </TimelineItem>
              ))}
          </Timeline>
        </Col>
        <Col span={11} id="commentsLayers">
          <div
            style={
              !showCommentLayout
                ? { display: "none" }
                : { marginTop: commentLayoutOffset + "px" }
            }
            className="comments-layer"
          >
            <List
              header={<div>当前意见</div>}
              itemLayout="horizontal"
              dataSource={comments || []}
              renderItem={(item) => (
                <li>
                  <Comment
                    key={uuid()}
                    author={item.userFullName}
                    avatar={item.userAvatar || "/default-user.png"}
                    content={
                      <div dangerouslySetInnerHTML={{ __html: item.content }} />
                    }
                    datetime={dayjs(item.createdTime).format(
                      "YYYY-MM-DD HH:mm:ss"
                    )}
                  />
                </li>
              )}
            />
            <Divider />

            {showCommentBtn && (
              <>
                {/* <Editor
                  onChange={(v) => setComment(v)}
                  mentions={mentionUsers || []}
                /> */}
                <Button
                  style={{ marginTop: 10 }}
                  loading={loading}
                  onClick={() => handleApproval(ActionType.Comment)}
                  type="primary"
                >
                  发表意见
                </Button>
              </>
            )}

            <Button
              style={{ marginTop: 10 }}
              onClick={() => {
                setShowCommentLayout(false);
                setShowCommentBtn(false);
                setComment("");
              }}
              type="default"
            >
              关闭
            </Button>
            {currentNode && checkStatus(currentNode) && (
              <div style={{ float: "right", marginTop: 10 }}>
                <Button
                  loading={loading}
                  onClick={() => handleApproval(ActionType.Approved)}
                >
                  同意
                </Button>
                <Button onClick={() => handleUpdate()}>退改</Button>
                <Button onClick={() => handleTrans()}>转办</Button>
                <Button
                  loading={loading}
                  onClick={() => handleApproval(ActionType.Rejected)}
                >
                  拒绝
                </Button>
              </div>
            )}
          </div>
        </Col>

        <Col span={9} className="comments-layers">
          <List
            itemLayout="horizontal"
            header="所有意见"
            dataSource={commentAll || []}
            renderItem={(item) => (
              <li>
                <Comment
                  key={uuid()}
                  author={item.userFullName}
                  avatar={item.userAvatar || "/default-user.png"}
                  content={
                    <div dangerouslySetInnerHTML={{ __html: item.content }} />
                  }
                  datetime={dayjs(item.createdTime).format(
                    "YYYY-MM-DD HH:mm:ss"
                  )}
                />
              </li>
            )}
          />
        </Col>
      </Row>
      <Modal
        title={`选择${addUserType === "approver" ? "审批人" : "抄送人"}`}
        visible={showUserChooser}
        maskClosable={false}
        onOk={handleApprovalUsers}
        onCancel={() => setShowUserChooser(false)}
      >
        <UserPicker
          title={`选择${addUserType === "approver" ? "审批人" : "抄送人"}`}
          dataSource={deptUsers}
          value={chooseUsers}
          onChange={(v) => {
            setChooseUsers(v);
          }}
        />
      </Modal>
      <Modal
        title="选择转办人"
        visible={showTransUserChooser}
        maskClosable={false}
        onOk={handleTransUser}
        onCancel={() => setShowTransUserChooser(false)}
      >
        <UserSinglePicker
          title="转办人"
          dataSource={zbUsers}
          value={chooseUser}
          onChange={(v) => {
            setChooseUser(v);
          }}
        />
      </Modal>
    </>
  );
};

export default ApprovalLine;

// 添加节点类型
export const OptionTypes = {
  APPROVAL: 'approval',
  CC: 'cc',
  BRANCH: 'branch',
  CONDITION: 'condition'
}
export const NodeTypes = { ...OptionTypes, START: 0 }
// 节点类型默认标题名
export const OptionNames = {
  [OptionTypes.APPROVAL]: '审批人',
  [OptionTypes.CC]: '抄送人',
  [OptionTypes.CONDITION]: '条件分支'
}
// 节点模板
export const NodeTemplates = {
  [OptionTypes.APPROVAL]: {
    title: '审批人',
    error: true,
    type: OptionTypes.APPROVAL,
    users: [],
    departments: []
  },
  [OptionTypes.CC]: {
    title: '抄送人',
    type: OptionTypes.CC,
    users: []
  },
  [OptionTypes.CONDITION]: {
    nodeName: '路由',
    type: OptionTypes.CONDITION,
    conditions: null,
    conditionNodes: []
  },
  [OptionTypes.BRANCH]: {
    title: '条件1',
    type: OptionTypes.CONDITION,
    conditions: {},
    nextNode: null
  }
}

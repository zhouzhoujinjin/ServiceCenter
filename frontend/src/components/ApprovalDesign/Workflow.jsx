/* eslint-disable default-case */
import React, { useEffect, useState } from 'react'
import EndNode from './Nodes/End'
import Render from './Nodes/Render'
import ZoomLayout from './ZoomLayout'
import { OptionTypes, NodeTemplates, NodeTypes } from './Nodes/consts'
import OperatorContext from './OperatorContext'
import './styles.less'
import { Form, Input, Modal, Select } from 'antd'
import ApprovalModal from './Nodes/Modals/ApprovalModal'
import CcModal from './Nodes/Modals/CcModal'
import ConditionModal from './Nodes/Modals/ConditionModal'

let currentNode = null

const Workflow = ({ startNode, fields, conditionFields, onSave }) => {
  let [config, setConfig] = useState()
  const [showApprovalModal, setShowApprovalModal] = useState(false)
  const [showConditionModal, setShowConditionModal] = useState(false)
  const [showCcModal, setShowCcModal] = useState(false)
  // const [showUserDepartmentModal, setShowUserDepartmentModal] = useState(false)

  useEffect(() => {
    if (onSave && config) {
      onSave({ ...config })
    }
  }, [config])

  const updateNode = () => {
    setConfig({ ...config })
    //onChange && onChange(config)
  }

  useEffect(() => {
    setConfig(startNode)
  }, [startNode])

  // 链表操作: 几种行为， 添加行为，删除行为，点击行为     pRef.nextNode -> objRef.nextNode -> 后继
  // 添加节点
  const onAddNode = (type, pRef, objRef) => {
    const o = objRef.nextNode
    if (type === OptionTypes.APPROVAL) {
      objRef.nextNode = { ...NodeTemplates[OptionTypes.APPROVAL], nextNode: o, conditionNodes: objRef.conditionNodes }
      objRef.conditionNodes = null
    } else if (type === OptionTypes.CC) {
      objRef.nextNode = { ...NodeTemplates[OptionTypes.CC], nextNode: o, conditionNodes: objRef.conditionNodes }
      objRef.conditionNodes = null
    } else if (type === OptionTypes.CONDITION) {
      const branchNode = {
        ...NodeTemplates[OptionTypes.BRANCH],
        title: '条件1',
        nextNode: o,
        conditionNodes: objRef.conditionNodes
      }
      objRef.conditionNodes = [branchNode, { ...NodeTemplates[OptionTypes.BRANCH], title: '条件2' }]
    } else if (type === OptionTypes.BRANCH) {
      objRef.conditionNodes.push({ ...NodeTemplates[NodeTypes.BRANCH] })
    }
    updateNode()
  }

  const onSwitchBranch = (objRef, index, swapIndex) => {
    objRef.conditionNodes[index] = objRef.conditionNodes.splice(swapIndex, 1, objRef.conditionNodes[index])[0]
    objRef.conditionNodes = [...objRef.conditionNodes]
    updateNode()
  }
  // 删除节点
  const onDeleteNode = (pRef, objRef, type, index) => {
    Modal.confirm({
      title: '是否删除节点？',
      onOk: () => {
        if (type === NodeTypes.BRANCH) {
          if (objRef.conditionNodes.length > 1) {
            objRef.conditionNodes.splice(index, 1)
          } else {
            const newObj = objRef.conditionNodes[0].nextNode
            objRef.nextNode = newObj
            objRef.conditionNodes = null
          }
        } else {
          const newObj = objRef.nextNode
          if (pRef) {
            pRef.nextNode = newObj
          }
        }
        updateNode()
      }
    })
  }

  // 获取节点
  const onSelectNode = (pRef, objRef) => {
    console.log(objRef)
    // console.log(fields)
    currentNode = {
      current: objRef,
      prev: pRef
    }
    switch (objRef.type) {
      case 'approval':
        setShowApprovalModal(true)
        setShowConditionModal(false)
        setShowCcModal(false)
        break
      case 'condition':
        setShowConditionModal(true)
        setShowApprovalModal(false)
        setShowCcModal(false)
        break
      case 'cc':
        setShowCcModal(true)
        setShowConditionModal(false)
        setShowApprovalModal(false)
        break
    }
  }

  return (
    <OperatorContext.Provider value={{ config, updateNode, onAddNode, onSwitchBranch, onDeleteNode, onSelectNode }}>
      <section className='dingflow-design'>
        <ZoomLayout>
          <Render config={config} />
          <EndNode />
        </ZoomLayout>
      </section>
      <ApprovalModal
        node={currentNode && currentNode.current}
        visible={showApprovalModal}
        onOk={(values) => {
          const { userIds, counterSign, users } = values
          currentNode.current.userIds = userIds
          currentNode.current.users = users
          currentNode.current.counterSign = counterSign
          updateNode()
          currentNode = null
          setShowApprovalModal(false)
        }}
        onCancel={() => {
          currentNode = null
          setShowApprovalModal(false)
        }}
      />
      <ConditionModal
        node={currentNode && currentNode.current}
        visible={showConditionModal}
        fields={fields}
        conditionFields={conditionFields}
        onOk={(values) => {
          const { conditions, content } = values
          currentNode.current.conditions = conditions
          currentNode.current.content = content
          updateNode()
          currentNode = null
          setShowConditionModal(false)
        }}
        onCancel={() => {
          currentNode = null
          setShowConditionModal(false)
        }}
      />
      <CcModal
        node={currentNode && currentNode.current}
        visible={showCcModal}
        onOk={(values) => {
          const { userIds, users } = values
          currentNode.current.userIds = userIds
          currentNode.current.users = users
          updateNode()
          currentNode = null
          setShowCcModal(false)
        }}
        onCancel={() => {
          currentNode = null
          setShowCcModal(false)
        }}
      />
    </OperatorContext.Provider>
  )
}

export default Workflow

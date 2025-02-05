import { Form, Modal, Radio, Select, Switch } from 'antd'
import React, { useEffect, useState } from 'react'
import UserSelect from '../../../UserSelect'

const ApprovalModal = (props) => {
  const { node, onOk, onCancel, visible } = props
  const [selectedUsers, setSelectedUsers] = useState([])
  const [switchEnable, setSwitchEnable] = useState(true)
  const [form] = Form.useForm()

  useEffect(() => {
    if (visible && node) {
      form.setFieldsValue({
        userIds: node.userIds,
        counterSign: node.counterSign
      })
      if (node.userIds.length > 1) {
        setSwitchEnable(true)
      } else {
        setSwitchEnable(false)
      }
    }
  }, [form.setFieldsValue, node, form, visible])

  const handleOk = () => {
    if (onOk) {
      const values = form.getFieldsValue()
      const info = { userIds: values.userIds, users: selectedUsers, counterSign: values.counterSign }
      onOk(info)
    }
  }

  const handleCancel = () => {
    onCancel && onCancel()
  }

  return (
    <Modal forceRender visible={visible} title='编辑审批节点' onOk={handleOk} onCancel={handleCancel}>
      <Form form={form}>
        <Form.Item label='审批方式' name='approvalType'>
          <Radio.Group>
            <Radio.Button value='user'>人员</Radio.Button>
            <Radio.Button value='manager' disabled>
              上级领导
            </Radio.Button>
            <Radio.Button value='department' disabled>
              部门
            </Radio.Button>
          </Radio.Group>
        </Form.Item>
        <Form.Item label='人员选择' name='userIds' rules={[{ required: true }]}>
          <UserSelect
            mode='multiple'
            onSelectedUsers={(users) => {
              setSelectedUsers(users)
              setSwitchEnable(users.length > 1)
            }}
          />
        </Form.Item>
        <Form.Item name='counterSign' label='是否会签' valuePropName='checked'>
          <Switch disabled={!switchEnable} checkedChildren='会签' unCheckedChildren='或签' />
        </Form.Item>
      </Form>
    </Modal>
  )
}

export default ApprovalModal

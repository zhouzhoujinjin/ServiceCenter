import { Form, Modal, Select } from 'antd'
import React, { useEffect, useState } from 'react'
import UserSelect from '../../../UserSelect'

const validateMessages = {
  required: '必填要素'
}
const CcModal = (props) => {
  const { node, onOk, onCancel, visible } = props
  const [selectedUsers, setSelectedUsers] = useState([])

  const [form] = Form.useForm()

  useEffect(() => {
    if (visible && node) {
      form.setFieldsValue({
        userIds: node.userIds
      })
    }
  }, [form.setFieldsValue, node, form, visible])

  const handleOk = () => {
    if (onOk) {
      const values = form.getFieldsValue()
      const info = { userIds: values.userIds, users: selectedUsers }
      onOk(info)
    }
  }

  const handleCancel = () => {
    onCancel && onCancel()
  }

  return (
    <Modal forceRender visible={visible} title='编辑抄送节点' onOk={handleOk} onCancel={handleCancel}>
      <Form form={form}>
        <Form.Item label='人员选择' name='userIds' rules={[{ required: true }]}>
          <UserSelect
            mode='multiple'
            onSelectedUsers={(users) => {
              setSelectedUsers(users)
            }}
          />
        </Form.Item>
      </Form>
    </Modal>
  )
}

export default CcModal

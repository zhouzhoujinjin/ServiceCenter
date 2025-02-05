/* eslint-disable react-hooks/exhaustive-deps */
/* eslint-disable default-case */
import React, { useCallback, useEffect, useState } from 'react'
import { Col, Form, Input, Modal, Radio, Row, Select, Switch, DatePicker, InputNumber, Divider } from 'antd'
import UserSelect from '../../../UserSelect'
import dayjs from 'dayjs'

const { RangePicker } = DatePicker
const validateMessages = {
  required: '必填要素'
}

const ConditionModal = (props) => {
  const { conditionFields, fields, node, onCancel, onOk, visible } = props
  const [formItems, setFormItems] = useState([])
  const [selectedConditions, setSelectedConditions] = useState([])
  const [form] = Form.useForm()
  const [selectedUsers, setSelectedUsers] = useState([])

  const renderFieldFormItem = (conditionField) => {
    switch (conditionField.valueType) {
      case 'number':
        return (
          <Form.Item key={conditionField.name} label={conditionField.title} name={conditionField.name}>
            <Input.Group>
              <Row gutter={9}>
                <Col span={4}>
                  <Form.Item name={[conditionField.name, 'min']}>
                    <Input />
                  </Form.Item>
                </Col>
                -
                <Col span={4}>
                  <Form.Item name={[conditionField.name, 'max']}>
                    <Input />
                  </Form.Item>
                </Col>
              </Row>
            </Input.Group>
          </Form.Item>
        )
      case 'date':
        return (
          <Form.Item key={conditionField.name} label={conditionField.title} name={conditionField.name} help='日期范围'>
            <RangePicker />
          </Form.Item>
        )
      case 'string':
        if (conditionField.controlType === 'multi-picker') {
          return (
            <Form.Item
              key={conditionField.name}
              label={conditionField.title}
              name={conditionField.name}
              rules={[{ required: true }]}>
              <Select mode='multiple'>
                {conditionField.controlOptions.options.map((o) => (
                  <Select.Option key={o} value={o}>
                    o
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          )
        }
        if (conditionField.controlType === 'single-picker') {
          return (
            <Form.Item
              key={conditionField.name}
              label={conditionField.title}
              name={conditionField.name}
              rules={[{ required: true }]}>
              <Select>
                {conditionField.controlOptions.options.map((o) => (
                  <Select.Option key={o} value={o}>
                    {o}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          )
        }
        if (conditionField.controlType === 'input') {
          return (
            <Form.Item
              key={conditionField.name}
              label={conditionField.title}
              name={conditionField.name}
              rules={[{ required: true }]}>
              <Input />
            </Form.Item>
          )
        }
    }
  }

  const renderApplicantFormItem = (applicantFieldName) => {
    switch (applicantFieldName) {
      case 'creatorId':
        return (
          <Form.Item key={applicantFieldName} label='人员选择' name='creatorId' rules={[{ required: true }]}>
            <UserSelect
              mode='multiple'
              onInit={(users) => setSelectedUsers(users)}
              onSelectedUsers={(users = []) => {
                setSelectedUsers(users)
              }}
            />
          </Form.Item>
        )
    }
  }

  const encodeCondition = (key, value) => {
    const commaPosition = value.indexOf(',')
    if (commaPosition > -1 && value.indexOf('in') === -1) {
      const [min, max] = value.split(',')
      if (isNaN(min) && !isNaN(Date.parse(min)) && isNaN(max) && !isNaN(Date.parse(max))) {
        return { [key]: [dayjs(min).format('YYYY-MM-DD'), dayjs(max).format('YYYY-MM-DD')] }
      } else {
        return {
          [key]: {
            min: min,
            max: max
          }
        }
      }
    } else if (value.indexOf('in') > -1) {
      const values = value
        .replace('in ', '')
        .split(',')
        .map((v) => (!isNaN(v) ? parseInt(v) : v))
      return {
        [key]: values
      }
    } else if (value.indexOf('=') > -1) {
      return { [key]: value }
    }
  }

  const decodeCondition = (values) => {
    //条件节点中文内容
    let content = ''
    //条件节点判断内容
    let conditions = {}
    Object.keys(values).map((k) => {
      const conditionTitle = conditionFields.find((cf) => cf.name === k).title

      if (typeof values[k] === 'string') {
        conditions[k] = values[k]
        content += `<p>${conditionTitle}: ${values[k]}</p>`
      } else {
        if (Array.isArray(values[k])) {
          conditions[k] = `in ${values[k].join(',')}`
          if (k === 'creatorId') {
            content += `<p>${conditionTitle}: ${values[k]
              .map((v) => selectedUsers.find((u) => u.id === v).profiles.fullName)
              .join(', ')}</p>`
          } else {
            content += `<p>${conditionTitle}: ${values[k].join(', ')}</p>`
          }
        }
        if ('min' in values[k] || 'max' in values[k]) {
          const { min, max } = values[k]
          conditions[k] = `${min || ''},${max || ''}`
          content += `<p>${conditionTitle}: ${min || 0} - ${max || '∞'}</p>`
        }
      }
    })
    return {
      conditions: conditions,
      content: content
    }
  }

  const refreshForm = (keys) => {
    //从ConditionFields中获取选中的内容
    const selectedFields = conditionFields.filter((c) => keys.indexOf(c.name) > -1)
    const formItems = selectedFields.map((s) => {
      //通过类型判断使用那种渲染方式，并返回组件数组
      switch (s.type) {
        case 'field':
          const formField = fields.find((x) => x.name === s.name)
          return renderFieldFormItem(formField)
        case 'applicant':
          return renderApplicantFormItem(s.name)
      }
    })
    setFormItems(formItems)
  }

  useEffect(() => {
    if (visible && node) {
      const keys = Object.keys(node.conditions)
      if (keys) {
        setSelectedConditions(keys)
        refreshForm(keys)
        const fieldsValue = keys.map((k) => {
          //将条件字符转为对象
          const obj = encodeCondition(k, node.conditions[k])
          return { ...obj }
        })
        let values = {}
        fieldsValue.forEach((f) => {
          //将编译后的条件数组转为formFieldValues的对象
          for (const [key, value] of Object.entries(f)) {
            values[key] = value
          }
        })
        form.setFieldsValue({ ...values })
      }
    }
  }, [form.setFieldsValue, node, form, fields, visible, conditionFields])

  const handleOk = async () => {
    form
      .validateFields()
      .then((values) => {
        const result = decodeCondition(values)
        if (onOk) {
          onOk({ ...result })
        }
      })
      .catch((errorInfo) => {
        console.log(errorInfo)
      })
  }

  const handleCancel = () => {
    onCancel && onCancel()
  }

  const handleOnChange = (v) => {
    setSelectedConditions(v)
    refreshForm(v)
  }

  return (
    <Modal forceRender visible={visible} title='编辑条件节点' onOk={handleOk} onCancel={handleCancel}>
      <Row>
        <Select mode='multiple' style={{ width: '500px' }} value={selectedConditions} onChange={handleOnChange}>
          {conditionFields &&
            conditionFields.map((c) => (
              <Select.Option key={c.name} value={c.name}>
                {c.title}
              </Select.Option>
            ))}
        </Select>
      </Row>
      <Divider>编辑值</Divider>
      <Row>
        <Form form={form} validateMessages={validateMessages}>
          {formItems}
        </Form>
      </Row>
    </Modal>
  )
}

export default ConditionModal

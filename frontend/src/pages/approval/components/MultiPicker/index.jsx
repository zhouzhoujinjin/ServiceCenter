import React, { useState, useEffect } from 'react'
import { Select } from 'antd'

const { Option } = Select

const MultiPicker = ({ title, range, value, onChange }) => {
  const [options] = useState(range || [])
  const [selected, setSelected] = useState(undefined)

  useEffect(() => {
    setSelected(value)
  }, [value, options])

  const onSelectChange = (v) => {
    if (onChange) {
      onChange(v)
    }
  }

  return (
    <Select
      mode='multiple'
      style={{ width: '100%' }}
      value={selected}
      dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
      placeholder={`请选择${title}`}
      allowClear
      onChange={onSelectChange}>
      {options &&
        options.map((o) => (
          <Option key={o} value={o}>
            {o}
          </Option>
        ))}
    </Select>
  )
}

export default MultiPicker

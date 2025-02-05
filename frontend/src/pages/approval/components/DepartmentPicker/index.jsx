import React, { useState, useEffect } from 'react'
import { Select } from 'antd'

const { Option } = Select

const DepartmentPicker = ({ name, range, value, onChange }) => {
  const [options, setOptions] = useState(range || [])
  const [selected, setSelected] = useState(undefined)

  useEffect(() => {
    setOptions(range)
  }, [range])

  useEffect(() => {
    const r = (value && value.map((o) => o.id)) || []
    setSelected(r)
  }, [value, options])

  const onSelectChange = (v) => {
    if (onChange) {
      let a = [v]
      const r = a.map((o) => ({ id: o, title: options && options.find((t) => t.id === o).title }))
      onChange(r)
    }
  }

  return (
    <Select
      style={{ width: '100%' }}
      value={selected}
      dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
      placeholder={`请选择 ${name}`}
      onChange={onSelectChange}
    >
      {options &&
        options.map((o) => (
          <Option key={o.id} value={o.id}>
            {o.title}
          </Option>
        ))}
    </Select>
  )
}

export default DepartmentPicker

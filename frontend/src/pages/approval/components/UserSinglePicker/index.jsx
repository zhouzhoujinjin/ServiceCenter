import React, { useState, useEffect } from 'react'
import { TreeSelect } from 'antd'

const UserSinglePicker = ({ onChange, value, dataSource, title }) => {
  const [options, setOptions] = useState([])
  const [selected, setSelected] = useState([])

  useEffect(() => {
    setOptions(dataSource)
  }, [dataSource])

  useEffect(() => {
    const r = (value && value.id) || []
    setSelected(r)
  }, [value, options])

  const onSelectChange = (v) => {
    if (onChange) {
      let users = []
      options.map((l) => l.children).forEach((p) => p.forEach((m) => users.push(m)))
      const r = {
        id: v,
        name: options && users.find((t) => t.value === v).title,
        avatar: options && users.find((t) => t.value === v).avatar
      }
      onChange(r)
    }
  }

  return (
    <TreeSelect
      style={{ width: '100%' }}
      value={selected}
      dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
      placeholder={`请选择${title}`}
      allowClear
      treeData={options || []}
      onChange={onSelectChange}></TreeSelect>
  )
}

export default UserSinglePicker

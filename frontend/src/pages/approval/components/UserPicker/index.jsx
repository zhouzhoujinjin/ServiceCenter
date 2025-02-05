import React, { useState, useEffect } from 'react'
import { TreeSelect } from 'antd'

const UserPicker = ({ onChange, value, dataSource, title }) => {
  const [options, setOptions] = useState([])
  const [selected, setSelected] = useState([])

  useEffect(() => {
    let all = [{ title: '全选', value: '-1', children: dataSource }]
    setOptions(all)
  }, [dataSource])

  useEffect(() => {
    const r = (value && value.map((o) => o.id)) || []
    setSelected(r)
  }, [value, options])

  const onSelectChange = (v) => {
    if (onChange) {
      let users = []
      options[0].children.map((l) => l.children).forEach((p) => p.forEach((m) => users.push(m)))
      const r = v.map((o) => ({
        id: o,
        name: options && users.find((t) => t.value === o).title,
        avatar: options && users.find((t) => t.value === o).avatar
      }))
      onChange(r)
    }
  }

  return (
    <>
      <TreeSelect
        style={{ width: '100%' }}
        value={selected}
        dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
        placeholder={`请选择${title}`}
        allowClear
        treeCheckable={true}
        treeData={options || []}
        treeDefaultExpandedKeys={['-1']}
        onChange={onSelectChange}></TreeSelect>
    </>
  )
}

export default UserPicker

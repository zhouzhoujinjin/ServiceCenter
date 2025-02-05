import React, { useState, useEffect } from 'react'
import { Cascader } from 'antd'

const loop = (arr, cb) => {
  return arr.map((x) => {
    const y = cb(x)
    if (x.children && x.children.length > 0) {
      y.children = loop(x.children, cb)
    }

    return y
  })
}

const CascaderPicker = ({ title, range, value, onChange }) => {
  const [options, setOptions] = useState(range || [])
  const [selected, setSelected] = useState(undefined)

  useEffect(() => {
    if (range && range.length) {
      // debugger
      setOptions(
        loop(range, (x) => {
          if (typeof x === 'string') {
            return { label: x, value: x }
          } else if (!x.value) {
            x.value = x.label
            return { ...x }
          }
        })
      )
    }
  }, [range])

  useEffect(() => {
    setSelected(value ? value.split('/') : [])
  }, [value, options])

  const onSelectChange = (v) => {
    if (onChange) {
      onChange(v.join('/'))
    }
  }

  return (
    <Cascader
      style={{ width: 300 }}
      value={selected}
      dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
      placeholder={`请选择${title}`}
      allowClear
      options={options}
      onChange={onSelectChange}
    ></Cascader>
  )
}

export default CascaderPicker

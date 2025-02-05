import React, { useState, useEffect } from 'react'
import { InputNumber } from 'antd'

const InputNumber1 = ({ value, onChange, min, max, step }) => {
  const [selected, setSelected] = useState(undefined)

  useEffect(() => {
    setSelected(value)
  }, [value])

  const onSelectChange = (v) => {
    if (onChange) {
      onChange(v)
    }
  }

  return (
    <InputNumber
      style={{ width: 200 }}
      min={min || 0}
      max={max || 1000}
      step={step || 1}
      value={selected}
      onChange={onSelectChange}
    />
  )
}

export default InputNumber1

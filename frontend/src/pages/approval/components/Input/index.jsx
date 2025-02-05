import React, { useState, useEffect } from 'react'
import { Input } from 'antd'

const Input1 = ({ value, onChange, addonBefore, addonAfter, memo, options }) => {
  const [selected, setSelected] = useState(undefined)

  useEffect(() => {
    setSelected(value)
  }, [value])

  const onSelectChange = (e) => {
    if (onChange) {
      onChange(e.target.value)
    }
  }

  return (
    <>
      <Input addonBefore={addonBefore || ''} addonAfter={addonAfter || ''} value={selected} disabled={options.disabled} onChange={onSelectChange} />
      <div style={{ color: 'red' }}> {memo || null} </div>
    </>
  )
}

export default Input1

import React, { useState, useEffect, useRef } from 'react'
import { Input } from 'antd'

const { TextArea } = Input

const TextArea1 = ({ value, onChange, rows, memo }) => {
  const [internalValue, setInternalValue] = useState(value)

  useEffect(() => {
    setInternalValue(value)
  }, [value])

  const onSelectChange = (e) => {
    if (onChange) {
      onChange(e.target.value)
    }
  }

  return (
    <>
      <TextArea rows={rows || 4} onChange={onSelectChange} value={internalValue} />
      {/* <div style={{ color: 'red' }}> {memo || null} </div> */}
    </>
  )
}

export default TextArea1

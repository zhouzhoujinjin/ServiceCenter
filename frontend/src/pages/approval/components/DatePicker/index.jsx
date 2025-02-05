import React, { useState, useEffect } from 'react'
import { DatePicker } from 'antd'
import dayjs from 'dayjs'

const DatePicker1 = ({ value, onChange }) => {
  const [selected, setSelected] = useState(undefined)

  useEffect(() => {
    setSelected(value ? dayjs(value) : null)
  }, [value])

  const onSelectChange = (v) => {
    if (onChange) {
      onChange(dayjs(v).format('YYYY-MM-DD'))
    }
  }

  return <DatePicker style={{ width: 200 }} value={selected} onChange={onSelectChange} />
}

export default DatePicker1

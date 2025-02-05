import React, { useState, useEffect } from 'react'
import { DatePicker } from 'antd'
import dayjs from 'dayjs'

const DateTimePicker = ({ value, onChange }) => {
  const [selected, setSelected] = useState(undefined)

  useEffect(() => {
    setSelected(value ? dayjs(value) : null)
  }, [value])

  const onSelectChange = (v) => {
    if (onChange) {
      onChange(v ? dayjs(v).format('YYYY-MM-DD HH:mm') : null)
    }
  }

  return (
    <DatePicker
      style={{ width: 200 }}
      showTime={{ hideDisabledOptions: true, format: 'HH:mm' }}
      value={selected}
      onChange={onSelectChange}
    />
  )
}

export default DateTimePicker

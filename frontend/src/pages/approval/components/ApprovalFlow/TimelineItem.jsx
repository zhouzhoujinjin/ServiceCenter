import React, { useState, useEffect } from 'react'

import { Timeline } from 'antd'
import './styles.less'

const TimelineItem = (props) => {
  const { title, extras, children, col } = props
  const [groups, setGroups] = useState([])

  useEffect(() => {
    const temp = col || 5
    if (children && Array.isArray(children) && children.length > temp) {
      let t = []
      let p = []
      let i = 1
      for (let index = 0; index < children.length; index++) {
        const e = children[index]
        if (i % temp === 0) {
          p = []
          i++
        } else {
          p.push(e)
          if (i < children.length) {
            i++
          } else {
            t.push(<div className='timeline-item-content'>{p}</div>)
          }
          if (i % temp === 0) {
            p.push(children[index + 1])
            t.push(<div className='timeline-item-content'>{p}</div>)
          }
        }
      }
      setGroups(t)
    } else {
      setGroups(<div className='timeline-item-content'>{children}</div>)
    }
  }, [children, col])

  return (
    <Timeline.Item className='timeline-item'>
      <div className='timeline-item-head'>
        <div className='timeline-item-title'>{title}</div>
        {extras && <div className='timeline-item-extras'>{extras}</div>}
      </div>
      {groups}
    </Timeline.Item>
  )
}

export default TimelineItem

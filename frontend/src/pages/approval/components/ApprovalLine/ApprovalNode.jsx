import React from 'react'
import { Badge, Avatar } from 'antd'

const ApprovalNode = ({ avatar, badgeIcon, title, onClick, className }) => {
  return (
    <Badge
      count={badgeIcon && <img src={badgeIcon} alt='' />}
      className={className}
      onClick={() => onClick && onClick()}>
      <Avatar src={avatar} size='large' shape='square' />
      <div className='avatar-field-name'>{title}</div>
    </Badge>
  )
}
export default ApprovalNode

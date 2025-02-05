import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { Tag } from 'antd'
import React from 'react'
import NodeWrapper from './NodeWrapper'

const formatApplicants = ({ departments = [], users = [] }) => {
  const deptBadges =
    departments.map((x) => (
      <Tag color='blue' key={x.id}>
        {x.title}
      </Tag>
    )) || []
  const userBadges =
    users.map((x) => (
      <Tag color='green' key={x.id}>
        {x.profiles && x.profiles.fullName ? x.profiles.fullName : x.userName}
      </Tag>
    )) || []
  const badges = [...deptBadges, ...userBadges]
  return badges
}
const StartNode = (props) => {
  return (
    <NodeWrapper
      type={0}
      objRef={props.objRef}
      onContentClick={props.onContentClick}
      title={<span>{props.title}</span>}>
      <div className='text'>{formatApplicants(props.applicants || {}) || '所有人'}</div>
      <FontAwesomeIcon icon={['fal', 'chevron-right']} />
    </NodeWrapper>
  )
}
export default StartNode

import React, { useContext } from 'react'
import NodeWrapper from './NodeWrapper'
import TitleElement from './TitleElement'
import OperatorContext from '../OperatorContext'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'

const ApproverNode = (props) => {
  const { onDeleteNode, onSelectNode } = useContext(OperatorContext)
  const delNode = () => {
    onDeleteNode(props.pRef, props.objRef)
  }
  const onChange = (val) => {
    // 数据设置不用去重新渲染
    props.pRef.nextNode.title = val
  }

  const onContentClick = () => {
    onSelectNode(props.pRef, props.objRef)
    props.onContentClick && props.onContentClick()
  }

  return (
    <NodeWrapper
      titleStyle={{ backgroundColor: 'rgb(255, 148, 62)' }}
      onContentClick={onContentClick}
      title={<TitleElement delNode={delNode} placeholder='审核人' title={props.title} onTitleChange={onChange} />}
      objRef={props.objRef}>
      <div className='text'>
        {props.users ? props.users.map((x) => x.profiles.fullName).join(', ') : '请选择审核人'}
      </div>
      <FontAwesomeIcon icon={['fal', 'chevron-right']} />
    </NodeWrapper>
  )
}
export default ApproverNode

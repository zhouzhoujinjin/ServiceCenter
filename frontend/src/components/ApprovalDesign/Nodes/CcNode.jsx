import React, { useContext } from 'react'
import NodeWrapper from './NodeWrapper'
import TitleElement from './TitleElement'
import OperatorContext from '../OperatorContext'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
const CcNode = (props) => {
  const { onDeleteNode, onSelectNode } = useContext(OperatorContext)
  function delNode() {
    onDeleteNode(props.pRef, props.objRef)
  }
  function onChange(val) {
    props.pRef.nextNode.title = val
  }

  function onContentClick() {
    onSelectNode(props.pRef, props.objRef)
    props.onContentClick && props.onContentClick()
  }

  let TitleEl = <TitleElement delNode={delNode} placeholder='抄送人' title={props.title} onTitleChange={onChange} />
  return (
    <NodeWrapper
      titleStyle={{ backgroundColor: 'rgb(50, 150, 250)' }}
      onContentClick={onContentClick}
      title={TitleEl}
      objRef={props.objRef}>
      <div className='text'>
        {props.users ? props.users.map((x) => x.profiles.fullName).join(', ') : '请选择抄送人'}
      </div>
      <FontAwesomeIcon icon={['fal', 'chevron-right']} />
    </NodeWrapper>
  )
}
export default CcNode

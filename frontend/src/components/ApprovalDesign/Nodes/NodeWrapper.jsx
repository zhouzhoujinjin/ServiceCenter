import React from 'react'
import { NodeTypes } from './consts'
import AddButton from './AddButton'

const NodeWrapper = (props) => {
  return (
    <div>
      <div className='node-wrap'>
        <div className={`node-wrap-box${props.type === NodeTypes.START ? ' start-node' : ''}`}>
          <div className='title' style={props.titleStyle}>
            {props.title}
          </div>
          <div className='content' onClick={props.onContentClick}>
            {props.children}
          </div>
        </div>
        <AddButton objRef={props.objRef} />
      </div>
    </div>
  )
}
export default NodeWrapper

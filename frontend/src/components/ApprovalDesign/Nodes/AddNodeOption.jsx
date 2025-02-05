import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import React from 'react'

const AddNodeOption = (props) => {
  return (
    <div className={'add-node-popover-item ' + props.type} onClick={props.onClick}>
      <div className='item-wrapper'>
        <FontAwesomeIcon
          icon={[
            'fal',
            props.type === 'approval' ? 'user-check' : props.type === 'cc' ? 'paper-plane' : 'network-wired'
          ]}
        />
      </div>
      <p>{props.name}</p>
    </div>
  )
}

export default AddNodeOption

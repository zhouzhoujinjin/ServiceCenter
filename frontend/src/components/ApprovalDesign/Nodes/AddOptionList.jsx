import React from 'react'
import AddNodeOption from './AddNodeOption'
import { OptionNames, OptionTypes } from './consts'

const AddNodeList = (props) => {
  return (
    <div className='add-node-popover-body'>
      <AddNodeOption
        type='approval'
        onClick={() => props.onOptionClick(OptionTypes.APPROVAL)}
        name={OptionNames[OptionTypes.APPROVAL]}
      />
      <AddNodeOption type='cc' onClick={() => props.onOptionClick(OptionTypes.CC)} name={OptionNames[OptionTypes.CC]} />
      <AddNodeOption
        type='condition'
        onClick={() => props.onOptionClick(OptionTypes.CONDITION)}
        name={OptionNames[OptionTypes.CONDITION]}
      />
    </div>
  )
}

export default AddNodeList

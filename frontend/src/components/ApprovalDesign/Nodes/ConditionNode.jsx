import React, { useContext } from 'react'
import AddButton from './AddButton'
import Render from './Render'
import { NodeTypes } from './consts'
import OperatorContext from '../OperatorContext'
import { Button } from 'antd'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
const CoverLine = ({ first = false, last = false }) => {
  return (
    <>
      {first && <div className='top-left-cover-line'></div>}
      {first && <div className='bottom-left-cover-line'></div>}
      {last && <div className='top-right-cover-line'></div>}
      {last && <div className='bottom-right-cover-line'></div>}
    </>
  )
}

const BranchNode = (props) => {
  const { first = false, last = false } = props
  const renderContent = () => {
    if (props.content) {
      return (
        <div
          dangerouslySetInnerHTML={{
            __html: props.content
          }}
        />
      )
    }
    if (props.conditions) {
      return (
        <div
          dangerouslySetInnerHTML={{
            __html: Object.keys(props.conditions)
              .map((x) => `${x}: ${props.conditions[x]}<br/>`)
              .join('')
          }}
        />
      )
    }
    return <span className='placeholder'>请设置条件</span>
  }
  return (
    <div className='condition-node'>
      <div className='condition-node-box'>
        <div className='auto-judge'>
          {!first && <div className='sort-left' onClick={props.sortLeft}></div>}
          <div className='title-wrapper'>
            <span className='editable-title'>{props.title || `条件${props.priorityLevel}`}</span>
            <span className='priority-title'>
              <Button.Group>
                <Button className='btn' disabled={first} type='link' onClick={props.onMoveLeft}>
                  <FontAwesomeIcon icon={['fal', 'chevron-left']} />
                </Button>
                <Button className='btn' disabled={last} type='link' onClick={props.onMoveRight}>
                  <FontAwesomeIcon icon={['fal', 'chevron-right']} />
                </Button>
                <Button className='btn' type='link'>
                  <FontAwesomeIcon icon={['fal', 'trash']} onClick={props.onDeleteBranch} />
                </Button>
              </Button.Group>
            </span>
          </div>
          {!last && <div className='sort-right' onClick={props.sortRight}></div>}
          <div className='content' onClick={() => props.onBranchClick(props.objRef)}>
            <div className='text'>{renderContent()}</div>
          </div>
        </div>
        <AddButton objRef={props.objRef} />
      </div>
    </div>
  )
}

const ConditionNode = ({ conditionNodes: branches = [], ...restProps }) => {
  const { onAddNode, onDeleteNode, onSelectNode, onSwitchBranch } = useContext(OperatorContext)
  const addBranch = () => {
    onAddNode(NodeTypes.BRANCH, restProps.pRef, restProps.objRef)
  }
  const onDeleteBranch = (index) => {
    onDeleteNode(restProps.pRef, restProps.objRef, NodeTypes.BRANCH, index)
  }
  const onBranchClick = (objRef) => {
    onSelectNode(restProps.objRef, objRef)
  }
  const onMoveLeft = (index) => {
    if (index > 0) {
      onSwitchBranch(restProps.objRef, index, index - 1)
    }
  }
  const onMoveRight = (index) => {
    if (index < branches.length - 1) {
      onSwitchBranch(restProps.objRef, index, index + 1)
    }
  }

  return (
    branches &&
    branches.length > 0 && (
      <div className='branch-wrap'>
        <div className='branch-box-wrap'>
          <div className='branch-box'>
            <button className='add-branch' onClick={addBranch}>
              添加条件
            </button>
            {branches.map((item, index) => {
              return (
                <div className='col-box' key={`${index}`}>
                  <BranchNode
                    {...item}
                    first={index === 0}
                    priorityLevel={index + 1}
                    onBranchClick={onBranchClick}
                    onMoveLeft={() => onMoveLeft(index)}
                    onMoveRight={() => onMoveRight(index)}
                    onDeleteBranch={() => onDeleteBranch(index)}
                    last={index === branches.length - 1}
                    objRef={item}
                  />
                  {item.conditionNodes && <ConditionNode {...item} objRef={item} pRef={item} />}
                  {item.nextNode && <Render pRef={item} config={item.nextNode} />}
                  <CoverLine first={index === 0} last={index === branches.length - 1} />
                </div>
              )
            })}
          </div>
          <AddButton objRef={restProps.objRef} />
        </div>
      </div>
    )
  )
}

export default ConditionNode

import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import React, { useState, useEffect, useRef } from 'react'
const TitleElement = (props) => {
  let [title, setTitle] = useState('')
  let [editable, setEditable] = useState(false)
  let input = useRef(null)
  useEffect(() => {
    setTitle(props.title || props.placeholder)
  }, [props.title, props.placeholder])

  function onFocus(e) {
    e.currentTarget.select()
  }
  function onBlur() {
    setEditable(false)
    if (!title) {
      setTitle(props.placeholder)
    }
  }
  function onClick() {
    setEditable(true)
  }
  useEffect(() => {
    if (editable) {
      input.current.focus()
    }
  }, [editable])
  function onChange(e) {
    let val = e.target.value
    props.onTitleChange && props.onTitleChange(val)
    setTitle(val)
  }
  return (
    <>
      {props.icon && <FontAwesomeIcon icon={props.icon} />}
      {editable ? (
        <input
          ref={input}
          type='text'
          className='ant-input editable-title-input'
          onBlur={onBlur}
          onChange={onChange}
          onFocus={onFocus}
          value={title}
          placeholder={props.placeholder}
        />
      ) : (
        <span className='editable-title' onClick={onClick}>
          {title}
        </span>
      )}
      <FontAwesomeIcon icon={['fal', 'trash']} onClick={props.delNode} />
    </>
  )
}
export default TitleElement

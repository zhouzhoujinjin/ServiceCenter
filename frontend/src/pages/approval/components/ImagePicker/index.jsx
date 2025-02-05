import React, { useState, useEffect } from 'react'
import { Upload } from 'antd'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'

const ImagePicker = ({ fileList, onChange }) => {
  const [files, setFiles] = useState([])

  useEffect(() => {
    setFiles(fileList)
  }, [fileList])

  const onSelectChange = (v) => {
    if (onChange) {
      onChange(v)
    }
  }

  return (
    <Upload
      name='image'
      listType='picture-card'
      fileList={files}
      onChange={onSelectChange}
      action={`/api/attach/approval/image`}>
      <div>
        {<FontAwesomeIcon icon='upload' />}
        <div className='ant-upload-text'>上传文件</div>
      </div>
    </Upload>
  )
}

export default ImagePicker

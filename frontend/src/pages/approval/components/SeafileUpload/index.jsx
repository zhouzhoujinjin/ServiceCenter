import React, { useState, useEffect } from 'react'
import { Upload, Button } from 'antd'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'

const SeafileUpload = ({ fileList, onChange, templateName, memo }) => {
  const [files, setFiles] = useState([])

  useEffect(() => {
    setFiles(fileList || [])
  }, [fileList])

  const onSelectChange = (v) => {
    console.log(v)
    if (onChange) {
      onChange(v)
    }
  }

  // const onBeforeUpload = (file) => {
  //   const ext = file.name.split('.').pop()
  //   if (!['docx', 'xlsx', 'pptx'].includes(ext)) {
  //     message.error('上传文件不符合要求')
  //     return false
  //   }
  //   return true
  // }

  return (
    <>
      <Upload
        name='file'
        listType='text'
        fileList={files}
        data={{ templateName }}
        onChange={onSelectChange}
        action={`/api/approval/upload`}
      >
        <Button>
          <FontAwesomeIcon icon='upload' /> 上传文档
        </Button>
      </Upload>
      <div style={{ color: 'grey' }}> {memo || null} </div>
    </>
  )
}

export default SeafileUpload

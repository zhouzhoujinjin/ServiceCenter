import React, { useState, useEffect, useCallback, Fragment } from 'react'
import { PageWrapper } from '../../components/PageWrapper'
import { Select, Button, Table, message, Input, Upload, Modal, Row, Col, Tag } from 'antd'
import {
  GetDeptUsers,
  GetApprovalVerifyItems,
  PublishVerifiedItem,
  GetPublishType,
  DeleteVerifiedFile,
  GetTemplateNames
} from './services/approval'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import dayjs from 'dayjs'
import { token } from '../../utils/token'
import { dotKeyToNested } from '../../utils/utils'
import UserPicker from '../Approval/components/UserPicker/index'

let uploadIndex = 0
//#region 上传图片
const uploadButton = (
  <div>
    <Button icon={<FontAwesomeIcon icon='upload' />}>上传校稿后待发布文件</Button>
  </div>
)
//#endregion

const generatorUploadItem = (url, title) => ({
  uid: (--uploadIndex).toString(),
  name: title,
  status: 'done',
  url
})

const ApprovalVerifyPage = () => {
  const [options, setOptions] = useState([])
  const [loading, setLoading] = useState(false)
  const [pagination, setPagination] = useState({ pageSize: 10 })
  const [currentPage, setCurrentPage] = useState(1)
  const [currentItem, setCurrentItem] = useState({})
  const [filters, setFilters] = useState({})
  const [showModal, setShowModal] = useState(false)
  const [showPublishTypeModal, setShowPublishTypeModal] = useState(false)
  const [verifiedFiles, setVerifiedFiles] = useState([])
  const [verifyList, setVerifyList] = useState([])
  const [publishTypes, setPublishTypes] = useState({})
  const [selectedPublishType, setSelectedPublishType] = useState('01')
  const [publishDept, setPublishDept] = useState('')
  const [publishTitle, setPublishTitle] = useState('')
  const [deptUsers, setDeptUsers] = useState([]) //所有人员
  const [chooseUsers, setChooseUsers] = useState([])

  const columns = [
    {
      title: '标题',
      dataIndex: 'title',
      width: '15%'
    },
    {
      title: '审批类型',
      dataIndex: 'templateTitle',
      width: '15%'
    },
    {
      title: '申请人',
      dataIndex: ['creator', 'profiles', 'fullName'],
      width: '15%'
    },
    {
      title: '申请部门',
      render: (text, record) => {
        const formFieldValues = dotKeyToNested(record.content)
        if (formFieldValues && formFieldValues.departments) {
          let departments = Object.keys(formFieldValues.departments).map((k) => formFieldValues.departments[k])
          return departments.map((x) => x.title).join(', ')
        }
        return ''
      },
      width: '15%'
    },
    {
      title: '创建日期',
      dataIndex: 'createdTime',
      width: '10%',
      render: (text) => {
        return dayjs(text).format('YYYY-MM-DD')
      }
    },
    {
      title: '是否发布',
      dataIndex: 'isPublished',
      width: '10%',
      render: (text) => {
        let label = text ? '是' : '否'
        let color = text ? 'green' : 'red'
        return <Tag color={color}>{label}</Tag>
      }
    },
    {
      title: '操作',
      render: (text, record) => (
        <Fragment>
          <>
            <Button
              type='link'
              onClick={() => {
                let temporaryDownloadLink = document.createElement('a')
                temporaryDownloadLink.style.display = 'none'
                document.body.appendChild(temporaryDownloadLink)
                record.finalFiles &&
                  record.finalFiles.forEach((f) => {
                    var download = f
                    temporaryDownloadLink.setAttribute('href', download.url)
                    temporaryDownloadLink.setAttribute('download', download.title)

                    temporaryDownloadLink.click()
                  })
                document.body.removeChild(temporaryDownloadLink)
              }}>
              下载终稿文件
            </Button>
            <Button
              type='link'
              onClick={() => {
                if (record.verifiedFiles && record.verifiedFiles.length > 0) {
                  setVerifiedFiles(record.verifiedFiles.map((file) => generatorUploadItem(file.url, file.title)))
                } else {
                  setVerifiedFiles([])
                }
                setShowModal(true)
                setCurrentItem(record)
              }}>
              上传待发布文件
            </Button>
            <Button
              type='primary'
              onClick={() => {
                setCurrentItem(record)
                setPublishTitle(record.title)
                setShowPublishTypeModal(true)
              }}>
              {record.isPublished ? '重新发布' : '正式发布'}
            </Button>
          </>
        </Fragment>
      )
    }
  ]
  const handleUploadEvent = (e, itemId) => {
    switch (e.file.status) {
      case 'done':
        message.success('上传成功!')
        refreshTable()
        break
      case 'removed':
        DeleteVerifiedFile(itemId, e.file.name, (data) => {
          message.success('删除成功!')
          refreshTable()
        })
        break
      default:
        break
    }
  }

  const handleTableChange = (pagination) => {
    setCurrentPage(pagination.current)
  }

  useEffect(() => {
    GetTemplateNames((data) => {
      setOptions(data.map((o) => ({ label: o.title, value: o.name })))
    })
    GetPublishType((data) => {
      setPublishTypes(data)
    })
    GetDeptUsers((data) => {
      setDeptUsers(data)
    })
  }, [])

  const refreshTable = useCallback(() => {
    setLoading(true)
    GetApprovalVerifyItems({ filters, page: currentPage, size: 10 }, (result) => {
      setLoading(false)
      setVerifyList(result.data)
      setPagination({
        pageSize: 10,
        total: result.total
      })
    })
  }, [currentPage, filters])

  useEffect(() => {
    refreshTable()
  }, [currentPage, filters, refreshTable])

  const handleTypeChange = () => {
    if (publishTitle === '') {
      message.error('发布标题必填')
      return
    }
    if (selectedPublishType && currentItem) {
      let users = chooseUsers && chooseUsers.map((x) => x.id)
      let purview = {
        publishType: selectedPublishType,
        publishDepartment: publishDept,
        publishTitle: publishTitle,
        purview: users || []
      }

      PublishVerifiedItem({ itemId: currentItem.id, purview }, (data) => {
        message.success('修改成功')
        setShowPublishTypeModal(false)
        refreshTable()
      })
    }
  }

  return (
    <>
      {showModal && (
        <Modal
          title={`${currentItem && currentItem.title} 发布校稿完成文件`}
          width={800}
          maskClosable={false}
          visible={true}
          onCancel={() => {
            setShowModal(false)
          }}
          footer={null}>
          <Row>
            <Upload
              name='file'
              multiple={false}
              listType='text'
              // className='avatar-uploader'
              showUploadList={true}
              onChange={(e) => {
                handleUploadEvent(e, currentItem.id)
              }}
              action={`/api/approval/verify/${currentItem.id}/upload`}
              defaultFileList={verifiedFiles || []}
              headers={{
                Authorization: `Bearer ${token.get()}`
              }}>
              {uploadButton}
            </Upload>
          </Row>
        </Modal>
      )}
      {showPublishTypeModal && (
        <Modal
          title='选择发布文件类型'
          width={800}
          visible={true}
          onCancel={() => {
            setShowPublishTypeModal(false)
          }}
          footer={[
            <Button
              key='back'
              onClick={() => {
                handleTypeChange()
              }}>
              发布
            </Button>
          ]}>
          <Row>
            <Col span={5}>发布类型</Col>
            <Col>
              <Select
                style={{ width: 240 }}
                onChange={(val) => {
                  setSelectedPublishType(val)
                }}>
                {publishTypes &&
                  Object.keys(publishTypes).map((k) => (
                    <Select.Option value={k} key={k}>
                      {publishTypes[k]}
                    </Select.Option>
                  ))}
              </Select>
            </Col>
          </Row>
          <Row style={{ marginTop: 10 }}>
            <Col span={5}>发布部门</Col>
            <Col span={19}>
              <Input
                style={{ width: 240 }}
                onChange={(e) => {
                  const v = e.target.value
                  if (v) {
                    setPublishDept(v)
                  } else {
                    setPublishDept('')
                  }
                }}
              />
            </Col>
          </Row>
          <Row style={{ marginTop: 10 }}>
            <Col span={5}>发布标题</Col>
            <Col span={19}>
              <Input
                style={{ width: 595 }}
                value={publishTitle}
                onChange={(e) => {
                  const v = e.target.value
                  if (v) {
                    setPublishTitle(v)
                  } else {
                    setPublishTitle('')
                  }
                }}
              />
            </Col>
          </Row>
          <Row style={{ marginTop: 10 }}>
            <Col span={5}>查看范围</Col>
            <Col span={19}>
              <UserPicker
                title={`选择查看范围`}
                dataSource={deptUsers}
                value={chooseUsers}
                onChange={(v) => {
                  setChooseUsers(v)
                }}
              />
            </Col>
          </Row>
        </Modal>
      )}
      <PageWrapper
        title='校稿审核'
        extras={
          <>
            <Select
              options={[{ label: '全部类型', value: '' }, ...options]}
              onChange={(v) => {
                setFilters((f) => ({ ...f, templateName: v }))
                setCurrentPage(1)
              }}
              style={{ width: 160 }}
              value={filters.templateName}
            />
            <Input
              style={{ width: 150 }}
              placeholder='检索关键字'
              onPressEnter={(e) => {
                const key = e.target.value
                setFilters((f) => ({ ...f, query: key }))
                setCurrentPage(1)
              }}
            />
          </>
        }>
        <Table
          loading={loading}
          dataSource={verifyList || []}
          columns={columns}
          rowKey='id'
          pagination={{ ...pagination, current: currentPage }}
          onChange={handleTableChange}
        />
      </PageWrapper>
    </>
  )
}

export default ApprovalVerifyPage

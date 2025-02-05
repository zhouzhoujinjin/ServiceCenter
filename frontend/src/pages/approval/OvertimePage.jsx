import React, { useState, useEffect, useCallback, Fragment } from 'react'
import { PageWrapper } from '../../components/PageWrapper'
import { useTabLayout } from '../../hooks/useTabLayout'
import { useHistory } from 'react-router-dom'
import { Button, Tag, Table, DatePicker, Form, Modal, message } from 'antd'
import { GetOvertimeItems, UpdateOvertimeFinishDate, ExportOvertime } from './services/approval'
import { formItemLayout } from '../../utils/formLayouts'
import dayjs from 'dayjs'

const { RangePicker } = DatePicker

const validateMessages = {
  required: '必填要素'
}

const OvertimePage = () => {
  const { addTab } = useTabLayout()
  const history = useHistory()
  const [loading, setLoading] = useState(false)
  const [pagination, setPagination] = useState({ pageSize: 20 })
  const [currentPage, setCurrentPage] = useState(1)
  const [list, setList] = useState([])
  const [filters, setFilters] = useState({ startDate: '', endDate: '' })

  const [visible, setVisible] = useState(false)
  const [chooseItem, setChooseItem] = useState(undefined)
  const [form] = Form.useForm()

  const columns = [
    {
      title: '编号',
      dataIndex: 'code',
      width: 100
    },
    {
      title: '标题',
      dataIndex: 'title'
    },
    {
      title: '开始时间',
      dataIndex: ['content', 'startDate'],
      width: 200
    },
    {
      title: '预计结束时间',
      dataIndex: ['content', 'endDate'],
      width: 200
    },
    {
      title: '实际结束时间',
      dataIndex: ['content', 'finishDate'],
      width: 200
    },
    {
      title: '时长(小时)',
      dataIndex: ['content', 'days'],
      width: 120
    },
    {
      title: '事由',
      dataIndex: ['content', 'description'],
      width: 120
    },
    {
      title: '创建人',
      dataIndex: ['creator', 'profiles', 'fullName'],
      width: 80
    },
    {
      title: '创建日期',
      dataIndex: 'createdTime',
      width: 120,
      render: (text) => {
        return dayjs(text).format('YYYY-MM-DD')
      }
    },
    {
      title: '状态',
      dataIndex: 'status',
      width: '10%',
      render: (text, record) => {
        const color =
          text === 1
            ? 'gold'
            : text === 2
            ? 'blue'
            : text === 3
            ? 'green'
            : text === 4
            ? 'red'
            : text === 5
            ? 'magenta'
            : 'geekblue'
        let info =
          text === 1
            ? '草稿'
            : text === 2
            ? '待审批'
            : text === 3
            ? '通过'
            : text === 4
            ? '拒绝'
            : text === 5
            ? '取消'
            : '需要上传终稿'
        if ([2, 8, 9, 13].includes(record.templateId) && text === 6) {
          info = '上传盖章扫描件'
        }
        return <Tag color={color}>{info}</Tag>
      }
    },
    {
      title: '操作',
      render: (text, record) => (
        <Fragment>
          <Button
            type='link'
            onClick={() => {
              setChooseItem(record)
              setVisible(true)
            }}>
            补充
          </Button>
          <Button
            type='link'
            onClick={() => {
              addTab({
                key: `/approvals/${record.id}/info`,
                title: `查看${record.templateTitle}`,
                prev: '/approvals',
                closable: true
              })
              history.push(`/approvals/${record.id}/info`)
            }}>
            详细
          </Button>
        </Fragment>
      ),
      width: 250
    }
  ]

  const refreshTable = useCallback(() => {
    setLoading(true)
    GetOvertimeItems({ filters, page: currentPage, size: 20 }, (result) => {
      setList(result.data)
      setPagination({
        pageSize: 20,
        total: result.total
      })
      setLoading(false)
    })
  }, [currentPage, filters])

  useEffect(() => {
    refreshTable()
  }, [currentPage, refreshTable])

  const handleTableChange = (pagination) => {
    setCurrentPage(pagination.current)
  }

  const handleSubmit = (values) => {
    if (values && chooseItem) {
      const finishDate = dayjs(values.finishDate).format('YYYY-MM-DD HH:mm')
      UpdateOvertimeFinishDate({ itemId: chooseItem.id, finishDate }, (data) => {
        if (data) {
          message.success('操作成功')
          setVisible(false)
        }
      })
    }
  }

  return (
    <PageWrapper
      title='加班管理'
      majorAction={null}
      extras={
        <>
          <RangePicker
            onChange={(_, dateStrings) => {
              setFilters((f) => ({ ...f, startDate: dateStrings[0], endDate: dateStrings[1] }))
            }}
          />
          <Button
            type='default'
            onClick={() => {
              ExportOvertime(filters, (data) => {
                window.open(data, 'aaaaa')
              })
            }}>
            导出
          </Button>
        </>
      }>
      <Table
        loading={loading}
        dataSource={list || []}
        columns={columns}
        rowKey='id'
        pagination={{ ...pagination, current: currentPage }}
        onChange={handleTableChange}
      />
      {visible && (
        <Modal
          title='更新实际结束时间'
          maskClosable={false}
          visible={visible}
          onOk={() => form.submit()}
          onCancel={() => {
            setVisible(false)
          }}
          width={1000}>
          <Form form={form} onFinish={handleSubmit} validateMessages={validateMessages}>
            <Form.Item label='实际结束时间' name='finishDate' rules={[{ required: true }]} {...formItemLayout}>
              <DatePicker showTime={{ hideDisabledOptions: true, format: 'HH:mm' }} />
            </Form.Item>
          </Form>
        </Modal>
      )}
    </PageWrapper>
  )
}

export default OvertimePage

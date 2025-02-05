import React, { useState, useEffect, useCallback, Fragment } from 'react'
import { PageWrapper } from '../../components/PageWrapper'
import { useTabLayout } from '../../hooks/useTabLayout'
import { useHistory } from 'react-router-dom'
import { Button, Tag, Table, DatePicker, Select } from 'antd'
import { GetLeaveItems, ExportLeave } from './services/approval'
import dayjs from 'dayjs'

const { RangePicker } = DatePicker
const { Option } = Select

const LeavePage = () => {
  const { addTab } = useTabLayout()
  const history = useHistory()
  const [loading, setLoading] = useState(false)
  const [pagination, setPagination] = useState({ pageSize: 20 })
  const [currentPage, setCurrentPage] = useState(1)
  const [list, setList] = useState([])
  const [filters, setFilters] = useState({ startDate: '', endDate: '', type: 'leave', title: '请假统计' })

  const leaveCols = [
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
      title: '请假开始时间',
      dataIndex: ['content', 'startDate'],
      width: 200
    },
    {
      title: '请假结束时间',
      dataIndex: ['content', 'endDate'],
      width: 200
    },
    {
      title: '请假类型',
      dataIndex: ['content', 'leaveType'],
      width: 120
    },
    {
      title: '时长(小时)',
      dataIndex: ['content', 'days'],
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
      width: 150
    }
  ]

  const outCols = [
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
      title: '外出预计时间',
      dataIndex: ['content', 'startDate'],
      width: 120
    },
    {
      title: '返回时间',
      dataIndex: ['content', 'returnDate'],
      width: 120
    },
    {
      title: '事由',
      dataIndex: ['content', 'description']
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
      width: 150
    }
  ]

  const refreshTable = useCallback(() => {
    setLoading(true)
    GetLeaveItems({ filters, page: currentPage, size: 20 }, (result) => {
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

  return (
    <PageWrapper
      title={filters.title}
      majorAction={null}
      extras={
        <>
          <Select
            style={{ width: 100 }}
            value={filters.type}
            onChange={(v) => setFilters((f) => ({ ...f, type: v, title: v === 'leave' ? '请假统计' : '外出统计' }))}>
            <Option value='leave'>请假</Option>
            <Option value='out'>外出</Option>
          </Select>
          <RangePicker
            onChange={(_, dateStrings) => {
              setFilters((f) => ({ ...f, startDate: dateStrings[0], endDate: dateStrings[1] }))
            }}
          />
          <Button
            type='default'
            onClick={() => {
              ExportLeave(filters, (data) => {
                window.open(data, 'aaaaa')
              })
            }}>
            {' '}
            导出
          </Button>
        </>
      }>
      <Table
        loading={loading}
        dataSource={list || []}
        columns={filters.type === 'leave' ? leaveCols : outCols}
        rowKey='id'
        pagination={{ ...pagination, current: currentPage }}
        onChange={handleTableChange}
      />
    </PageWrapper>
  )
}

export default LeavePage

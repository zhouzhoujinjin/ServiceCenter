import React, { useState, useEffect, useCallback, Fragment } from 'react'
import { PageWrapper } from '../../components/PageWrapper'
import { Table, Button, Input, Radio } from 'antd'
import { useTabLayout } from '../../hooks/useTabLayout'
import { useHistory } from 'react-router-dom'
import { GetApprovalTemplates, /*GetUserApprovalCounts,*/ GetApprovals } from './services/approval'
import dayjs from 'dayjs'

const ApprovalsSendPage = () => {
  //const [templates, setTemplates] = useState([])
  const [options, setOptions] = useState([])
  //const [counts, setCounts] = useState({})
  const { addTab } = useTabLayout()
  const history = useHistory()
  const [loading, setLoading] = useState(false)
  const [pagination, setPagination] = useState({ pageSize: 20 })
  const [currentPage, setCurrentPage] = useState(1)
  const [list, setList] = useState([])
  const [filters, setFilters] = useState({ actionType: 'cc' })

  const columns = [
    {
      title: '标题',
      dataIndex: 'title'
    },
    {
      title: '审批类型',
      dataIndex: 'templateTitle',
      width: '15%'
    },
    {
      title: '创建人',
      width: '10%',
      dataIndex: ['creator', 'profiles', 'fullName']
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
      width: 250
    }
  ]

  useEffect(() => {
    GetApprovalTemplates((data) => {
      //setTemplates(data)
      let v = data.map((o) => ({ label: o.title, value: o.name }))
      setOptions(v)
    })
  }, [])

  // useEffect(() => {
  //   GetUserApprovalCounts((data) => {
  //     setCounts(data)
  //   })
  // }, [templates])

  const refreshTable = useCallback(() => {
    setLoading(true)
    GetApprovals({ filters, page: currentPage, size: 20 }, (result) => {
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
      title='抄送我'
      extras={
        <>
          <Radio.Group
            onChange={(e) => {
              const k = e.target.value
              setFilters({ actionType: 'cc', templateName: k })
              setCurrentPage(1)
            }}
            value={filters.templateName}
            optionType='button'>
            {options.map((o) => (
              <Radio.Button key={o.value} value={o.value}>
                {o.label}
              </Radio.Button>
            ))}
          </Radio.Group>
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
        dataSource={list || []}
        columns={columns}
        rowKey='id'
        pagination={{ ...pagination, current: currentPage }}
        onChange={handleTableChange}
      />
    </PageWrapper>
  )
}

export default ApprovalsSendPage

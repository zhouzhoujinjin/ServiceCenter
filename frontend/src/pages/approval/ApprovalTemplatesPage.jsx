import { Button, Table, Image } from 'antd'
import React, { useEffect, useState } from 'react'
import { useHistory } from 'react-router'
import { PageWrapper } from '../../components/PageWrapper'
import { useTabLayout } from '../../hooks/useTabLayout'
import { GetTemplates } from './services/adminApproval'

const ApprovalTemplatesPage = () => {
  const [templates, setTemplates] = useState([])
  const { addTab } = useTabLayout()
  const history = useHistory()
  const handleDesign = (templateName, templateTitle) => {
    addTab({
      key: `/approvals/templates/${templateName}/design`,
      title: `设计 ${templateTitle}`,
      prev: '/approvals/templates',
      closable: true
    })
    history.push(`/approvals/templates/${templateName}/design`)
  }

  useEffect(() => {
    GetTemplates((data) => {
      data.sort((a, b) => (a.groupTitle < b.groupTitle ? -1 : 1))
      setTemplates(data)
    })
  }, [])
  const columns = [
    {
      title: '图标',
      dataIndex: 'iconUrl',
      render: (v) => <Image src={v} style={{ width: 32, height: 32 }} />,
      width: 60
    },
    {
      title: '名称',
      dataIndex: 'name',
      width: 140
    },
    {
      title: '标题',
      dataIndex: 'title',
      width: 140
    },
    {
      title: '介绍',
      dataIndex: 'description'
    },
    {
      title: '分组',
      dataIndex: 'groupTitle',
      width: 90
    },
    {
      title: '操作',
      render: (record) => (
        <>
          <Button onClick={() => handleDesign(record.name, record.title)}>设计</Button>
        </>
      ),
      width: 160
    }
  ]
  return (
    <PageWrapper title='流程模板列表'>
      <Table rowKey='name' columns={columns} dataSource={templates}></Table>
    </PageWrapper>
  )
}

export default ApprovalTemplatesPage

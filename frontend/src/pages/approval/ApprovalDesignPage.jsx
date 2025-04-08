import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { Button, Radio } from 'antd'
import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router'
import Workflow from '../../components/ApprovalDesign/Workflow'
import { PageWrapper } from '../../components/PageWrapper'
import { GetTemplate, UpdateTemplate } from './services/adminApproval'

const ApprovalDesignPage = (props) => {
  const [current, setCurrent] = useState('flow')
  const params = useParams()
  const templateName = params.templateName
  const [template, setTemplate] = useState()
  const [workFlowData, setWorkFlowData] = useState()

  useEffect(() => {
    GetTemplate(templateName, (data) => {
      setTemplate(data)
      setWorkFlowData(data.workflow.startNode)
    })
  }, [templateName])

  const handleSave = () => {
    // console.log(workFlowData)
    if (template) {
      UpdateTemplate(templateName, { startNode: workFlowData }, (data) => {
        // console.log(data)
      })
    }
  }

  return (
    <PageWrapper
      title='设计流程模板'
      majorAction={
        <Radio.Group value={current} onChange={(e) => setCurrent(e.target.value)}>
          <Radio.Button value='form'>表单</Radio.Button>
          <Radio.Button value='flow'>流程</Radio.Button>
        </Radio.Group>
      }
      extras={
        <Button onClick={handleSave}>
          <FontAwesomeIcon icon='save' /> 保存{current === 'form' ? '表单' : '流程'}
        </Button>
      }>
      {current === 'form' && '开发中...'}
      {current === 'flow' && (
        <div className='flow-wrapper' style={{ height: window.innerHeight - 142 }}>
          {template && (
            <Workflow
              startNode={template.workflow.startNode || { title: template.title, type: 'start' }}
              fields={template.fields}
              conditionFields={template.conditionFields || []}
              onSave={(data) => {
                setWorkFlowData(data)
              }}
            />
          )}
        </div>
      )}
    </PageWrapper>
  )
}

export default ApprovalDesignPage

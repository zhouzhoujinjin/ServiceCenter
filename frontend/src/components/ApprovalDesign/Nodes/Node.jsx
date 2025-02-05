import React from 'react'

import StartNode from './StartNode'
import ApproverNode from './ApproverNode'
import CcNode from './CcNode'
import ConditionNode from './ConditionNode'

const NodeMaps = {
  start: StartNode,
  approval: ApproverNode,
  cc: CcNode,
  condition: ConditionNode
}

export const MatchNode = ({ config, pRef }) => {
  const Node = NodeMaps[config.type] || null
  const majorNode = Node && <Node {...config} objRef={config} pRef={pRef} />
  return (
    <>
      {majorNode}
      {config.conditionNodes && <NodeMaps.condition {...config} objRef={config} pRef={pRef} />}
    </>
  )
}

export default MatchNode

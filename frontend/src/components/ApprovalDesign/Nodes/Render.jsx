import React from 'react'
import { MatchNode } from './Node'

const Render = ({ config, pRef }) => {
  return (
    <>
      {config && <MatchNode pRef={pRef} config={config} />}
      {config && config.nextNode && <Render pRef={pRef} config={config.nextNode} />}
    </>
  )
}

export default Render

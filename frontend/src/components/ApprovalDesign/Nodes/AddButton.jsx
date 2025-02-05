import React, { useState, useContext, useEffect } from "react";
import AddNodeList from "./AddOptionList";
import OperatorContext from "../OperatorContext";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Button } from "antd";
const AddButton = (props) => {
  let [showPop, setShowPop] = useState(false);
  function onClick() {
    setShowPop(!showPop);
  }
  const { onAddNode } = useContext(OperatorContext);
  const onOptionClick = (type) => {
    onAddNode(type, props.pRef, props.objRef);
    setShowPop(false);
  };

  // useEffect(() => {
  //   if (showPop) {
  //     const id = setTimeout(() => setShowPop(false), 3000)
  //     return () => clearTimeout(id)
  //   }
  // }, [showPop])

  return (
    <div className="add-node-btn-box">
      <div className="add-node-btn">
        {showPop && (
          <div
            className="add-popover"
            style={{ position: "absolute", zIndex: "10" }}
          >
            <AddNodeList onOptionClick={onOptionClick} />
          </div>
        )}
        <span>
          <Button
            className="btn"
            onClick={onClick}
            icon={<FontAwesomeIcon icon={["fal", "plus"]} />}
          ></Button>
        </span>
      </div>
    </div>
  );
};

export default AddButton;

import React from "react";
import "./Camera.css";

interface CameraFrameProps {
  title?: string;
  left?: number;
  top?: string | number;
}

const CameraFrame: React.FC<CameraFrameProps> = ({
  title = "◢ CAMERA FEED ◣",
  left =0,
  top = "0%",
}) => {
  return (
    <div
      className="camera-frame"
      style={{ left, top }}
    >
      <div className="camera-frame-corner corner-tl" />
      <div className="camera-frame-corner corner-tr" />
      <div className="camera-frame-corner corner-bl" />
      <div className="camera-frame-corner corner-br" />

      {/* <div className="camera-label">{title}</div> */}
    </div>
  );
};

export default CameraFrame;

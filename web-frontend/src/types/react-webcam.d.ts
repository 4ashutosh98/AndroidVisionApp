declare module 'react-webcam' {
  import React from 'react';

  export interface WebcamProps {
    audio?: boolean;
    audioConstraints?: MediaTrackConstraints;
    className?: string;
    height?: number | string;
    imageSmoothing?: boolean;
    mirrored?: boolean;
    minScreenshotHeight?: number;
    minScreenshotWidth?: number;
    onUserMedia?: (stream: MediaStream) => void;
    onUserMediaError?: (error: string | DOMException) => void;
    screenshotFormat?: 'image/webp' | 'image/png' | 'image/jpeg';
    screenshotQuality?: number;
    style?: React.CSSProperties;
    videoConstraints?: MediaTrackConstraints | boolean;
    width?: number | string;
  }

  export default class Webcam extends React.Component<WebcamProps> {
    getScreenshot(): string | null;
  }
}

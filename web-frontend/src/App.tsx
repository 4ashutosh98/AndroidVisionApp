import React, { useState, useEffect, useRef, useCallback } from 'react';
import Webcam from 'react-webcam';
import axios from 'axios';

// Replace with your backend URL - use localhost for development
const BACKEND_URL = 'http://localhost:5000';

const App: React.FC = () => {
  // State management
  const [handshake, setHandshake] = useState<'Connecting' | 'Connected' | 'Error'>('Connecting');
  const [currentView, setCurrentView] = useState<'scan' | 'camera' | 'preview' | 'results'>('scan');
  const [loading, setLoading] = useState(false);
  const [extractedText, setExtractedText] = useState<string>('');
  const [capturedImage, setCapturedImage] = useState<string | null>(null);
  const [apiError, setApiError] = useState<string | null>(null);

  const webcamRef = useRef<Webcam>(null);

  // Check backend health on mount
  useEffect(() => {
    console.log('[Frontend] Checking backend health...');
    axios.get(`${BACKEND_URL}/health`)
      .then(() => {
        console.log('[Frontend] Backend handshake success');
        setHandshake('Connected');
      })
      .catch(err => {
        console.error('[Frontend] Backend handshake failed', err);
        setHandshake('Error');
      });
  }, []);

  // Handle webcam capture
  const capturePhoto = useCallback(() => {
    if (webcamRef.current) {
      const imageSrc = webcamRef.current.getScreenshot();
      if (imageSrc) {
        setCapturedImage(imageSrc);
        setCurrentView('preview');
      }
    }
  }, [webcamRef]);

  // Send image to backend for processing
  const confirmAndProcess = async () => {
    if (!capturedImage) return;
    
    setLoading(true);
    setApiError(null);
    
    try {
      const formData = new FormData();
      
      // Convert base64 to blob
      const response = await fetch(capturedImage);
      const blob = await response.blob();
      formData.append('image', blob, 'nametag-scan.jpg');

      console.log('[Frontend] Sending vision request to backend...');
      const apiResponse = await axios.post(`${BACKEND_URL}/vision`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });

      console.log('[Frontend] Received response:', apiResponse.data);
      setExtractedText(apiResponse.data.result || 'No text found in image');
      setCurrentView('results');
      
    } catch (error: any) {
      console.error('[Frontend] Backend error:', error);
      setApiError('Failed to process image. Please check your connection and try again.');
    } finally {
      setLoading(false);
    }
  };

  // Navigation functions
  const startScan = () => {
    setCurrentView('camera');
    setApiError(null);
  };

  const goBack = () => {
    setCapturedImage(null);
    setExtractedText('');
    setApiError(null);
    setCurrentView('scan');
  };

  const rescan = () => {
    setCapturedImage(null);
    setApiError(null);
    setCurrentView('camera');
  };

  const backToPreview = () => {
    setExtractedText('');
    setApiError(null);
    setCurrentView('preview');
  };

  return (
    <div className="container">
      <h1 className="title">Vision AI</h1>
      <p className="subtitle">Nametag Scanner</p>
      
      {/* Backend status */}
      <div className={`health-status ${
        handshake === 'Connected' ? 'health-ok' : 
        handshake === 'Error' ? 'health-error' : 'health-connecting'
      }`}>
        Backend: {handshake}
      </div>

      {/* Main content based on current view */}
      {currentView === 'scan' && (
        <div className="capture-section">
          <button 
            className="camera-button"
            onClick={startScan}
            disabled={loading || handshake !== 'Connected'}
          >
            📷 Scan Nametag
          </button>
        </div>
      )}

      {currentView === 'camera' && (
        <div className="capture-section">
          <div className="webcam-container">
            <Webcam
              ref={webcamRef}
              audio={false}
              screenshotFormat="image/jpeg"
              className="webcam"
              videoConstraints={{
                facingMode: 'environment' // Use rear camera on mobile
              }}
            />
          </div>
          <div>
            <button 
              className="camera-button"
              onClick={capturePhoto}
              disabled={loading}
            >
              📸 Capture Photo
            </button>
            <button 
              className="clear-button"
              onClick={goBack}
            >
              ← Back
            </button>
          </div>
        </div>
      )}

      {currentView === 'preview' && capturedImage && (
        <div className="capture-section">
          <div>
            <img src={capturedImage} alt="Captured" className="preview-image" />
          </div>
          <div>
            <button 
              className="camera-button"
              onClick={confirmAndProcess}
              disabled={loading}
            >
              ✓ Confirm
            </button>
            <button 
              className="switch-button"
              onClick={rescan}
              disabled={loading}
            >
              🔄 Rescan
            </button>
            <button 
              className="clear-button"
              onClick={goBack}
            >
              ← Back
            </button>
          </div>
        </div>
      )}

      {currentView === 'results' && (
        <div className="capture-section">
          {capturedImage && (
            <div>
              <img src={capturedImage} alt="Scanned" className="preview-image" />
            </div>
          )}
          <div className="result-container">
            <h3 className="result-title">Extracted Text:</h3>
            <div className="result-text">
              {extractedText}
            </div>
            <div>
              <button 
                className="camera-button"
                onClick={goBack}
              >
                🔄 Scan Another
              </button>
              <button 
                className="clear-button"
                onClick={backToPreview}
              >
                ← Back to Preview
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Loading indicator */}
      {loading && (
        <div className="loading">
          <div className="spinner"></div>
          <p>Processing nametag with AI...</p>
        </div>
      )}

      {/* Error message */}
      {apiError && (
        <div className="error-message">
          {apiError}
        </div>
      )}
    </div>
  );
};

export default App;

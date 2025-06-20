import React, {useState, useEffect} from 'react';
import {ScrollView, Platform, StyleSheet, Text, ActivityIndicator, View} from 'react-native';
import {launchCamera, CameraOptions} from 'react-native-image-picker';
import axios from 'axios';

import {ButtonRow} from './components/ButtonRow';
import {TextBox} from './components/TextBox';
import {ImageViewer} from './components/ImageViewer';

// Replace with your dev machine IP reachable by Android device
const BACKEND_URL = 'http://10.0.0.207:3000';

const App = () => {
  // Health check handshake state
  const [handshake, setHandshake] = useState<'Connecting'|'Connected'|'Error'>('Connecting');
  const [text, setText] = useState<string>('');
  const [photoUri, setPhotoUri] = useState<string | null>(null);
  const [activeButton, setActiveButton] = useState<'ocr' | 'vision' | null>(null);
  const [loading, setLoading] = useState(false);
  const [apiError, setApiError] = useState<string | null>(null);

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

  // Capture photo and send to backend; mode is 'ocr' or 'vision'
  const captureAndSend = async (mode: 'ocr' | 'vision') => {
    if (Platform.OS !== 'android') return;
    setActiveButton(mode);
    setApiError(null);
    setLoading(true);
    try {
      console.log(`[Frontend] Sending ${mode} request to ${BACKEND_URL}/${mode}`);      const options: CameraOptions = {
        mediaType: 'photo',
        saveToPhotos: false,
        // Removed all compression settings to send full-quality images
      };
      const result = await launchCamera(options);
      if (result.assets && result.assets.length > 0) {
        const uri = result.assets[0].uri || '';
        setPhotoUri(uri);
        const form = new FormData();
        form.append('image', { uri, type: 'image/jpeg', name: 'photo.jpg' } as any);
        const res = await axios.post(`${BACKEND_URL}/${mode}`, form, {
          headers: { 'Content-Type': 'multipart/form-data' },
        });
        console.log('[Frontend] Received response:', res.data);
        const output = mode === 'ocr' ? res.data.text : res.data.result;
        setText(output);
      }
    } catch (e: any) {
      console.error('Backend error', e);
      setApiError('Failed to fetch from backend');
    } finally {
      setLoading(false);
    }
  };

  const clearPhoto = () => {
    setPhotoUri(null);
    setText('');
    setActiveButton(null);
  };

  return (
    <ScrollView contentInsetAdjustmentBehavior="automatic" style={styles.container}>
      <Text style={[styles.health, handshake === 'Connected' ? styles.healthOk : styles.healthErr]}>Backend: {handshake}</Text>
      {/* Loading indicator */}
      {loading && (
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color="#007AFF" />
          <Text>Waiting for model inference...</Text>
        </View>
      )}
      {/* API error message */}
      {apiError && <Text style={styles.errorText}>{apiError}</Text>}
      <ButtonRow
        onOcr={() => captureAndSend('ocr')}
        onVision={() => captureAndSend('vision')}
        activeButton={activeButton}
      />
      {photoUri && <ImageViewer uri={photoUri} onClear={clearPhoto} />}
      <TextBox text={text} />
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    paddingTop: 32,
    paddingHorizontal: 16,
  },
  health: {
    textAlign: 'center',
    marginBottom: 12,
    fontWeight: '500',
  },
  healthOk: { color: 'green' },
  healthErr: { color: 'red' },
  loadingContainer: {
    alignItems: 'center',
    marginBottom: 16,
  },
  errorText: {
    color: 'red',
    textAlign: 'center',
    marginBottom: 16,
  },
});

export default App;

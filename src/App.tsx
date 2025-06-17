import React, {useState} from 'react';
import {ScrollView, Platform, StyleSheet} from 'react-native';
import {launchCamera, CameraOptions} from 'react-native-image-picker';

import {ButtonRow} from './components/ButtonRow';
import {TextBox} from './components/TextBox';
import {ImageViewer} from './components/ImageViewer';

const App = () => {
  const [text, setText] = useState<string>('');
  const [photoUri, setPhotoUri] = useState<string | null>(null);

  const openCamera = async () => {
    if (Platform.OS !== 'android') return;
    const options: CameraOptions = {mediaType: 'photo', saveToPhotos: false};
    const result = await launchCamera(options);
    if (result.assets && result.assets.length > 0) {
      setPhotoUri(result.assets[0].uri || null);
    }
  };

  const clearPhoto = () => {
    setPhotoUri(null);
    setText('');
  };

  return (
    <ScrollView contentInsetAdjustmentBehavior="automatic" style={styles.container}>
      <ButtonRow onOcr={openCamera} onVision={openCamera} />
      <TextBox text={text} />
      {photoUri && <ImageViewer uri={photoUri} onClear={clearPhoto} />}
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 16,
  },
});

export default App;

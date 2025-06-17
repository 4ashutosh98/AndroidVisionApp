/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 */

import React, {useState} from 'react';
import {Button, Image, ScrollView, Text, View, StyleSheet, Platform} from 'react-native';
import {launchCamera, CameraOptions} from 'react-native-image-picker';

function App() {
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
      <View style={styles.buttonRow}>
        <Button title="OCR" onPress={openCamera} />
        <Button title="Vision LLM" onPress={openCamera} />
      </View>
      <View style={styles.textBox}>
        <Text>{text}</Text>
      </View>
      {photoUri && (
        <View style={styles.imageContainer}>
          <Image source={{uri: photoUri}} style={styles.image} />
          <Button title="Clear" onPress={clearPhoto} />
        </View>
      )}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {flex: 1, padding: 16},
  buttonRow: {flexDirection: 'row', justifyContent: 'space-between', marginBottom: 16},
  textBox: {height: 100, borderWidth: 1, borderColor: '#ccc', padding: 8, marginBottom: 16},
  imageContainer: {alignItems: 'center'},
  image: {width: '100%', height: 200, marginBottom: 8},
});

export default App;

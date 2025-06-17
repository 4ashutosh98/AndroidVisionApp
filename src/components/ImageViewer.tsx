import React from 'react';
import { View, Image, Button, StyleSheet } from 'react-native';

type ImageViewerProps = {
  uri: string;
  onClear: () => void;
};

export const ImageViewer: React.FC<ImageViewerProps> = ({ uri, onClear }) => (
  <View style={styles.container}>
    <Image source={{ uri }} style={styles.image} />
    <Button title="Clear" onPress={onClear} />
  </View>
);

const styles = StyleSheet.create({
  container: { alignItems: 'center' },
  image: { width: '100%', height: 200, marginBottom: 8 },
});

import React from 'react';
import { View, Button, StyleSheet } from 'react-native';

type ButtonRowProps = {
  onOcr: () => void;
  onVision: () => void;
};

export const ButtonRow: React.FC<ButtonRowProps> = ({ onOcr, onVision }) => (
  <View style={styles.row}>
    <Button title="OCR" onPress={onOcr} />
    <Button title="Vision LLM" onPress={onVision} />
  </View>
);

const styles = StyleSheet.create({
  row: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 16 },
});

import React from 'react';
import { View, Pressable, Text, StyleSheet } from 'react-native';

type ButtonRowProps = {
  onVision: () => void;
  activeButton: 'vision' | null;
};

export const ButtonRow: React.FC<ButtonRowProps> = ({ onVision, activeButton }) => (
  <View style={styles.row}>
    <Pressable
      onPress={onVision}
      style={[styles.button, activeButton === 'vision' && styles.activeButton]}
    >
      <Text style={styles.text}>Vision LLM</Text>
    </Pressable>
  </View>
);

const styles = StyleSheet.create({
  row: { flexDirection: 'row', marginBottom: 16 },
  button: {
    flex: 1,
    marginHorizontal: 4,
    paddingVertical: 12,
    backgroundColor: '#007AFF',
    alignItems: 'center',
    borderRadius: 4,
  },
  activeButton: {
    backgroundColor: 'pink',
  },
  text: {
    color: 'white',
    fontWeight: '600',
  },
});

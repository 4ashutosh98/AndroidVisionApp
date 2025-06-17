import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

type TextBoxProps = { text: string };

export const TextBox: React.FC<TextBoxProps> = ({ text }) => (
  <View style={styles.box}>
    <Text>{text}</Text>
  </View>
);

const styles = StyleSheet.create({
  box: { height: 100, borderWidth: 1, borderColor: '#ccc', padding: 8, marginBottom: 16 },
});

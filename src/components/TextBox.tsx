import React from 'react';
import { View, Text, ScrollView, StyleSheet } from 'react-native';

type TextBoxProps = { text: string };

export const TextBox: React.FC<TextBoxProps> = ({ text }) => (
  <View style={styles.container}>
    <ScrollView
      style={styles.box}
      contentContainerStyle={styles.contentContainer}
      showsVerticalScrollIndicator={true}
    >
      <Text style={styles.text}>{text}</Text>
    </ScrollView>
  </View>
);

const styles = StyleSheet.create({
  container: {
    marginTop: 16,
    flexGrow: 1,
  },
  box: {
    height: 400,
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#ccc',
    padding: 12,
  },
  contentContainer: {
    flexGrow: 1,
  },
  text: {
    color: '#000',
  },
});

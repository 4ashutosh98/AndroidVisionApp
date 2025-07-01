"""
Azure AI Vision Processing Script
Processes images using Azure's AI models for text extraction
"""

import json
import base64
import requests
import sys
import os

def process_image(image_base64, azure_endpoint, azure_key):
    """
    Process image using Azure AI API for text extraction
    
    Args:
        image_base64 (str): Base64 encoded image data
        azure_endpoint (str): Azure AI endpoint URL
        azure_key (str): Azure AI API key
        
    Returns:
        str: Extracted text from image
    """
    try:
        # Prepare the request
        data_url = f'data:image/jpeg;base64,{image_base64}'

        messages = [
            {
                'role': 'system',
                'content': 'You are a helpful assistant that can read and extract text from images using advanced vision understanding.'
            },
            {
                'role': 'user',
                'content': [
                    {
                        'type': 'text',
                        'text': 'Please extract all the text visible in this image. Only return the text content, nothing else.'
                    },
                    {
                        'type': 'image_url',
                        'image_url': {
                            'url': data_url
                        }
                    }
                ]
            }
        ]

        headers = {
            'Authorization': f'Bearer {azure_key}',
            'Content-Type': 'application/json'
        }

        payload = {
            'messages': messages,
            'model': 'gpt-4o',
            'temperature': 0.1,
            'max_tokens': 1000
        }

        response = requests.post(
            f'{azure_endpoint}/openai/deployments/gpt-4o/chat/completions?api-version=2024-02-15-preview',
            headers=headers,
            json=payload
        )

        if response.status_code != 200:
            raise Exception(f'Azure API error: {response.status_code} - {response.text}')

        result = response.json()
        extracted_text = result['choices'][0]['message']['content']
        
        return extracted_text
        
    except Exception as e:
        print(f"Error in Azure vision processing: {str(e)}", file=sys.stderr)
        raise

if __name__ == "__main__":
    # For direct execution/testing
    if len(sys.argv) != 4:
        print("Usage: python azure_vision.py <image_base64> <azure_endpoint> <azure_key>")
        sys.exit(1)
    
    image_base64 = sys.argv[1]
    azure_endpoint = sys.argv[2]
    azure_key = sys.argv[3]
    
    try:
        result = process_image(image_base64, azure_endpoint, azure_key)
        print(result)
    except Exception as e:
        print(f"Error: {str(e)}", file=sys.stderr)
        sys.exit(1)

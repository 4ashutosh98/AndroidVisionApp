"""
GitHub AI Vision Processing Script
Processes images using GitHub's AI models for text extraction
"""

import json
import base64
import requests
import sys
import os

def process_image(image_base64, github_token):
    """
    Process image using GitHub AI API for text extraction
    
    Args:
        image_base64 (str): Base64 encoded image data
        github_token (str): GitHub API token
        
    Returns:
        str: Extracted text from image
    """
    try:
        # Prepare the request
        data_url = f'data:image/jpeg;base64,{image_base64}'

        # Create messages for GitHub AI API (identical to Node.js implementation)
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

        # Make request to GitHub AI (replicating Node.js Azure AI Inference SDK call)
        headers = {
            'Authorization': f'Bearer {github_token}',
            'Content-Type': 'application/json'
        }

        payload = {
            'messages': messages,
            'model': 'meta/Llama-3.2-11B-Vision-Instruct',
            'temperature': 0.1,
            'max_tokens': 1000
        }

        response = requests.post(
            'https://models.github.ai/inference/chat/completions',
            headers=headers,
            json=payload
        )

        if response.status_code != 200:
            raise Exception(f'GitHub API error: {response.status_code} - {response.text}')

        result = response.json()
        extracted_text = result['choices'][0]['message']['content']
        
        return extracted_text
        
    except Exception as e:
        print(f"Error in GitHub vision processing: {str(e)}", file=sys.stderr)
        raise

if __name__ == "__main__":
    # For direct execution/testing
    if len(sys.argv) != 3:
        print("Usage: python github_vision.py <image_base64> <github_token>")
        sys.exit(1)
    
    image_base64 = sys.argv[1]
    github_token = sys.argv[2]
    
    try:
        result = process_image(image_base64, github_token)
        print(result)
    except Exception as e:
        print(f"Error: {str(e)}", file=sys.stderr)
        sys.exit(1)

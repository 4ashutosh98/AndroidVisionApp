"""
Image Utilities
Common utilities for image processing and handling
"""

import base64
import io
import sys
import json
from PIL import Image

def validate_image_base64(image_base64):
    """
    Validate that the provided base64 string is a valid image
    
    Args:
        image_base64 (str): Base64 encoded image data
        
    Returns:
        bool: True if valid image, False otherwise
    """
    try:
        # Remove data URL prefix if present
        if image_base64.startswith('data:image'):
            image_base64 = image_base64.split(',')[1]
        
        # Decode base64
        image_data = base64.b64decode(image_base64)
        
        # Try to open with PIL
        image = Image.open(io.BytesIO(image_data))
        image.verify()
        
        return True
    except Exception:
        return False

def get_image_info(image_base64):
    """
    Get basic information about an image
    
    Args:
        image_base64 (str): Base64 encoded image data
        
    Returns:
        dict: Image information (format, size, mode)
    """
    try:
        # Remove data URL prefix if present
        if image_base64.startswith('data:image'):
            image_base64 = image_base64.split(',')[1]
        
        # Decode base64
        image_data = base64.b64decode(image_base64)
        
        # Open with PIL
        image = Image.open(io.BytesIO(image_data))
        
        return {
            'format': image.format,
            'size': image.size,
            'mode': image.mode,
            'width': image.width,
            'height': image.height
        }
    except Exception as e:
        raise Exception(f"Error getting image info: {str(e)}")

def resize_image_if_needed(image_base64, max_width=1024, max_height=1024):
    """
    Resize image if it's larger than specified dimensions
    
    Args:
        image_base64 (str): Base64 encoded image data
        max_width (int): Maximum width
        max_height (int): Maximum height
        
    Returns:
        str: Base64 encoded resized image data
    """
    try:
        # Remove data URL prefix if present
        data_url_prefix = ""
        if image_base64.startswith('data:image'):
            parts = image_base64.split(',')
            data_url_prefix = parts[0] + ','
            image_base64 = parts[1]
        
        # Decode base64
        image_data = base64.b64decode(image_base64)
        
        # Open with PIL
        image = Image.open(io.BytesIO(image_data))
        
        # Check if resize is needed
        if image.width <= max_width and image.height <= max_height:
            return data_url_prefix + image_base64 if data_url_prefix else image_base64
        
        # Calculate new size maintaining aspect ratio
        ratio = min(max_width / image.width, max_height / image.height)
        new_size = (int(image.width * ratio), int(image.height * ratio))
        
        # Resize image
        resized_image = image.resize(new_size, Image.Resampling.LANCZOS)
        
        # Convert back to base64
        output = io.BytesIO()
        resized_image.save(output, format=image.format or 'JPEG')
        resized_base64 = base64.b64encode(output.getvalue()).decode('utf-8')
        
        return data_url_prefix + resized_base64 if data_url_prefix else resized_base64
        
    except Exception as e:
        raise Exception(f"Error resizing image: {str(e)}")

if __name__ == "__main__":
    # For testing utilities
    if len(sys.argv) < 2:
        print("Usage: python image_utils.py <command> [args...]")
        print("Commands:")
        print("  validate <image_base64>")
        print("  info <image_base64>")
        print("  resize <image_base64> [max_width] [max_height]")
        sys.exit(1)
    
    command = sys.argv[1]
    
    try:
        if command == "validate" and len(sys.argv) == 3:
            result = validate_image_base64(sys.argv[2])
            print(json.dumps({"valid": result}))
        elif command == "info" and len(sys.argv) == 3:
            result = get_image_info(sys.argv[2])
            print(json.dumps(result))
        elif command == "resize" and len(sys.argv) >= 3:
            max_width = int(sys.argv[3]) if len(sys.argv) > 3 else 1024
            max_height = int(sys.argv[4]) if len(sys.argv) > 4 else 1024
            result = resize_image_if_needed(sys.argv[2], max_width, max_height)
            print(result)
        else:
            print("Invalid command or arguments")
            sys.exit(1)
    except Exception as e:
        print(f"Error: {str(e)}", file=sys.stderr)
        sys.exit(1)

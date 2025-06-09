import cv2
import os
import numpy as np

# List of image file names to resize
image_names = ["house4.5.png"]  # Add your image names here

# Input and output folders
input_folder = r"D:\Projects\catday\OgImages"
output_folder = r"D:\Projects\catday\finImages"

# Max dimension size (e.g., 1080 pixels)
max_dim = 1024

# Create output folder if it doesn't exist
os.makedirs(output_folder, exist_ok=True)

for image_name in image_names:
    image_path = os.path.join(input_folder, image_name)

    # Load image with alpha channel
    img = cv2.imread(image_path, cv2.IMREAD_UNCHANGED)

    if img is None:
        print(f"Failed to load image: {image_name}")
        continue

    # Ensure image has alpha channel
    if img.shape[2] < 4:
        print(f"Image {image_name} does not have an alpha channel.")
        continue

    # Extract alpha channel
    alpha = img[:, :, 3]

    # Find bounding box of non-transparent pixels
    coords = cv2.findNonZero(alpha)
    if coords is None:
        print(f"No non-transparent pixels in {image_name}")
        continue
    x, y, w, h = cv2.boundingRect(coords)

    # Crop to bounding box
    cropped = img[y:y+h, x:x+w]

    # Resize logic
    height, width = cropped.shape[:2]
    print(f"Cropped dimensions: {width}x{height}")

    scale = max_dim / max(height, width)
    print(f"Scale factor: {scale:.2f}")
    if scale >= 1.0:
        resized = cropped
    else:
        new_width = int(width * scale)
        new_height = int(height * scale)
        resized = cv2.resize(cropped, (new_width, new_height), interpolation=cv2.INTER_AREA)

    # Save resized image preserving alpha
    output_path = os.path.join(output_folder, image_name)
    cv2.imwrite(output_path, resized)

    print(f"Processed {image_name} → {output_path} ({resized.shape[1]}x{resized.shape[0]})")

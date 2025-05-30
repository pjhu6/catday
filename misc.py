import cv2
import os

# Folder path
folder_path = r"D:\Projects\My project (1)\cropped"

# Supported image extensions (add more if needed)
image_extensions = {'.jpg', '.jpeg', '.png', '.bmp', '.tiff', '.tif', '.gif'}

# Iterate over files in folder
for filename in os.listdir(folder_path):
    # Check if file extension is an image type
    ext = os.path.splitext(filename)[1].lower()
    if ext in image_extensions:
        file_path = os.path.join(folder_path, filename)
        # Read image using OpenCV
        img = cv2.imread(file_path)
        if img is None:
            print(f"Failed to load image: {filename}")
            continue
        height, width = img.shape[:2]
        print(f"{filename}: {width}x{height}")
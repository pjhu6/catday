import cv2
import os

def crop_top_right(image_path, crop_width, output_path=None):
    # Load the image
    image = cv2.imread(image_path)
    if image is None:
        raise ValueError("Could not load the image. Check the path.")

    original_height, original_width = image.shape[:2]

    # Ensure crop_width is not larger than the image width
    crop_width = min(crop_width, original_width)

    # Calculate the crop height to maintain aspect ratio
    aspect_ratio = original_height / original_width
    crop_height = int(crop_width * aspect_ratio)

    # Ensure the crop height does not exceed the image height
    crop_height = min(crop_height, original_height)

    # Define the top-right crop box
    x_start = original_width - crop_width
    y_start = 0
    x_end = original_width
    y_end = crop_height

    # Crop the image
    cropped = image[y_start:y_end, x_start:x_end]

    # Show the cropped image
    # cv2.imshow("Cropped Top-Right", cropped)
    # cv2.waitKey(0)
    # cv2.destroyAllWindows()

    # Optionally save the cropped image
    if output_path:
        cv2.imwrite(output_path, cropped)

# Example usage
image_names = ["catday6.png", "catday5.png"]  # Add your image names here
input_directory = r"C:\Users\Patrick\Downloads\catdays"
output_directory = r"C:\Users\Patrick\Downloads\catdays\cropped"

for image_name in image_names:
    crop_top_right(os.path.join(input_directory, image_name), crop_width=1350, output_path=os.path.join(output_directory, image_name))

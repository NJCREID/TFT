import os
import requests
from PIL import Image

def download_item_images(url):
    response = requests.get(url)
    data = response.json()

    if "items" in data:
        for augment in data["items"]:
            key = augment["ingameKey"]
            image_url = augment["imageUrl"]

            if not image_url.startswith(('http://', 'https://')):
                  image_url = 'https:' + image_url

            image_response = requests.get(image_url, stream=True)
            if image_response.status_code == 200:
                try:
                    # Open the image and resize it to 64x64
                    image = Image.open(image_response.raw)
                    image = image.resize((64, 64))

                    # Extract the image file type from the URL
                    file_type = image_url.split(".")[-1]

                    output_path = f"../../../TFT API/TFT_API/assets/images/items/{key}.{file_type}"
                    os.makedirs(os.path.dirname(output_path), exist_ok=True)
                    image.save(output_path)
                    print(f"Image saved for augment: {key}")
                except Exception as e:
                    print(f"Failed to process image for item: {key}, URL: {image_url}, Error: {e}")
            else:
                print(f"Failed to download image for item: {key}")

# URL to fetch augment data
url = "https://tft.dakgg.io/api/v1/data/items?hl=en&season=set11"

# Call the function to download augment images
download_item_images(url)

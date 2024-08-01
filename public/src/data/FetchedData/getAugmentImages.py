import os
import requests

def rename_augment_images(url):
    response = requests.get(url)
    data = response.json()

    if "augments" in data:
        for augment in data["augments"]:
            key = augment["key"]
            ingame_key = augment["ingameKey"]

            folder_path = "../../../TFT API/TFT_API/assets/images/augments"
            current_image_path = os.path.join(folder_path, f"{key}.png")
            new_image_path = os.path.join(folder_path, f"{ingame_key}.png")

            if os.path.exists(current_image_path):
                try:
                    os.rename(current_image_path, new_image_path)
                    print(f"Renamed {current_image_path} to {new_image_path}")
                except Exception as e:
                    print(f"Failed to rename image for augment: {key}, Error: {e}")
            else:
                print(f"Image for {key} not found in augments folder")

# URL to fetch augment data
url = "https://tft.dakgg.io/api/v1/data/augments?hl=en&season=set11"

# Call the function to rename augment images
rename_augment_images(url)

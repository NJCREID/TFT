import requests
import json
import re
def fetch_and_process_items():
    url = "https://tft.dakgg.io/api/v1/data/augments?hl=en&season=set11"
    response = requests.get(url)

    if response.status_code == 200:
        data = response.json()
        augments = data.get("augments", [])

        processed_augments = []
        for augment in augments:
            
            # Remove unwanted fields
            augment["inGameKey"] = augment.pop("ingameKey")
            if "tags" in augment:
              for tag in augment["tags"]:
                  if(tag == "hidden"):
                      augment["isHidden"] = True
            augment.pop("championIngameKey", None)
            augment.pop("legendCodes", None)
            augment.pop("tags", None)
            augment.pop("isNew", None)
            # Set the name (with spaces removed) as the image URL
            augment["imageUrl"] = f"https://localhost:7235/api/image/augments/{augment["inGameKey"]}"
            
            processed_augments.append(augment)

        return processed_augments

def save_to_json(data, filename):
    with open(filename, 'w', encoding='utf-8') as json_file:
        json.dump(data, json_file, ensure_ascii=False, indent=4)

def main():
    processed_items = fetch_and_process_items()
    if processed_items:
        save_to_json({"augments": processed_items}, "augments.json")
        print("Data has been written to items.json")

if __name__ == "__main__":
    main()

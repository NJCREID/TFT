import requests
import json
import re

def fetch_and_process_items():
    url = "https://tft.dakgg.io/api/v1/data/items?hl=en&season=set11"
    response = requests.get(url)

    if response.status_code == 200:
        data = response.json()
        items = data.get("items", [])

        processed_items = []
        for item in items:
            
            if item.get("key") == "ExaltedEmblem":
                continue
            # Remove unwanted fields
            item.pop("ingameIcon", None)
            item.pop("fromDesc", None)
            item.pop("isUnique", None)
            item.pop("isSupport", None)
            item.pop("isArtifact", None)
            item.pop("isNew", None)
            item.pop("isNormal", None)
            item.pop("isRadiant", None)
            item["inGameKey"] = item.pop("ingameKey")
            # Set the name (with spaces removed) as the image URL
            item["imageUrl"] = f"https://localhost:7235/api/image/items/{item["inGameKey"]}"

            for key, value in item.items():
                if isinstance(value, str):
                    item[key] = value.replace('\u200b', '')
            if "tags" in item:
              for tag in item["tags"]:
                  if(tag == "hidden"):
                      item["isHidden"] = True        
    
            # Rename 'compositions' to 'recipe'
            if 'compositions' in item:
                item['recipe'] = item.pop('compositions')

            # Rename 'isFromItem' to 'isComponent'
            if 'isFromItem' in item:
                item['isComponent'] = item.pop('isFromItem')

            if 'tags' not in item:
                if 'Support item' in item.get('desc', ''):
                    item['tags'] = ['support']
                else:
                    item['tags'] = ['artifact']

            processed_items.append(item)

        tag_order = [
            "fromitem", "normal", "radiant", "artifact", "support", "storyweaver", "inkshadow", "unique"
        ]

        # Sort items based on the order of tags provided
        sorted_items = sorted(processed_items, key=lambda x: min(tag_order.index(tag) if tag in tag_order else float('inf') for tag in x.get("tags", ["unique"])))

        return sorted_items
    else:
        print(f"Failed to fetch data. Status code: {response.status_code}")
        return []

def save_to_json(data, filename):
    with open(filename, 'w', encoding='utf-8') as json_file:
        json.dump(data, json_file, ensure_ascii=False, indent=4)

def main():
    processed_items = fetch_and_process_items()
    if processed_items:
        save_to_json({"items": processed_items}, "items.json")
        print("Data has been written to items.json")

if __name__ == "__main__":
    main()

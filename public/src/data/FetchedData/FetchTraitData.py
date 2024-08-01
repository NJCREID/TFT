import requests
import json
import re

def fetch_and_process_items():
    url = "https://tft.dakgg.io/api/v1/data/traits?hl=en&season=set11"
    response = requests.get(url)

    if response.status_code == 200:
        data = response.json()
        traits = data.get("traits", [])

        processed_traits = []
        for trait in traits:
            trait["inGameKey"] = trait.pop("ingameKey")
            trait["tiers"] = trait.pop("stageStyles")
            for tier in trait["tiers"]:
                tier["level"] = tier.pop("min")
                tier["rarity"] = 1 if tier["style"] == "bronze" else 2 if tier["style"] == "silver" else 3 if tier["style"] == "gold" else 4 if tier["style"] == "chromatic" else None
                tier["desc"] = trait["stats"][str(tier["level"])]
                tier.pop("style", None)
                tier.pop("max", None)
            trait["tierString"] = " / ".join(trait["stats"].keys())
            trait["imageUrl"] = f"https://localhost:7235/api/image/traits/{trait["inGameKey"]}"
            trait.pop("blackImageUrl", None)
            trait.pop("whiteImageUrl", None)
            trait.pop("styles", None)
            trait.pop("type", None)
            trait.pop("stats", None)
            processed_traits.append(trait)

        return processed_traits
def save_to_json(data, filename):
    with open(filename, 'w', encoding='utf-8') as json_file:
        json.dump(data, json_file, ensure_ascii=False, indent=4)

def main():
    processed_items = fetch_and_process_items()
    if processed_items:
        save_to_json({"traits": processed_items}, "traits.json")
        print("Data has been written to items.json")

if __name__ == "__main__":
    main()

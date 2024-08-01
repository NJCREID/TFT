import requests
import json

def fetch_and_process_champions():
    url = "https://tft.dakgg.io/api/v1/data/champions?hl=en&season=set11"
    response = requests.get(url)

    if response.status_code == 200:
        data = response.json()
        champions = data.get("champions", [])

        processed_champions = []
        for champion in champions:
            if champion.get('isHiddenGuide') and champion['key'] != 'Kayle':
                continue
            if champion['key'] == 'Kayle':
                champion.pop("isHiddenGuide", None)
                champion.pop("isHiddenLanding", None)

            # Remove unwanted fields
            champion["inGameKey"] = champion.pop("ingameKey")
            champion["recommendedItems"] = champion.pop("recommendItems")
            champion.pop("tags", None)

            # Rename and change URLs
            if 'splashUrl' in champion:
                champion.pop("splashUrl", None)
            champion['splashImageUrl'] = f"https://localhost:7235/api/image/champions/splash/{champion['inGameKey']}"
            
            if 'imageUrl' in champion:
                champion['imageUrl'] = f"https://localhost:7235/api/image/champions/tiles/{champion['inGameKey']}"
            
            if 'originalImageUrl' in champion:
                champion.pop("originalImageUrl", None)
            champion['centeredImageUrl'] = f"https://localhost:7235/api/image/champions/centered/{champion['inGameKey']}"

            if 'health' in champion:
                champion['health'] = [int(round(h)) for h in champion['health']]

            if 'cost' in champion:
                champion['tier'] = champion['cost'][0] if champion['cost'] else None
            processed_champions.append(champion)

        return processed_champions
    else:
        print(f"Failed to fetch data. Status code: {response.status_code}")
        return []

def save_to_json(data, filename):
    with open(filename, 'w', encoding='utf-8') as json_file:
        json.dump(data, json_file, ensure_ascii=False, indent=4)

def main():
    processed_champions = fetch_and_process_champions()
    if processed_champions:
        save_to_json({"champions": processed_champions}, "champions.json")
        print("Data has been written to champions.json")

if __name__ == "__main__":
    main()

import requests
import json

# Base URL
BASE_URL = "https://tft.dakgg.io/api/v1/data/{type}?hl=en&season=set11"

# Types to fetch
types = ["augments", "traits", "items", "champions"]

# Path to JSON file
json_file_path = 'names.json'

# Function to fetch data and update JSON file
def fetch_and_update_json():
    # Dictionary to hold data
    names_data = {}

    for type_ in types:
        # Make the request to the API
        url = BASE_URL.format(type=type_)
        print(f"Fetching data for type: {type_}")
        response = requests.get(url)
        
        if response.status_code == 200:
            data = response.json()
            
            # Extract the relevant information and update the dictionary
            for item in data[type_]:
                ingame_key = item['ingameKey']
                name = item['name']
                
                # Add or update the item in the dictionary with name and type
                if ingame_key in names_data:
                    names_data[ingame_key].update({'name': name, 'type': type_})
                else:
                    names_data[ingame_key] = {'name': name, 'type': type_}
        else:
            print(f"Failed to fetch data for type {type_}: {response.status_code}")

    # Write updated data back to JSON file
    with open(json_file_path, 'w') as file:
        json.dump(names_data, file, indent=4)

if __name__ == "__main__":
    fetch_and_update_json()

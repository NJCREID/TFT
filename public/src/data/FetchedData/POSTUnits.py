import requests
import json

with open('champions.json', 'r') as file:
    data = json.load(file)

base_url = 'https://localhost:7235/api/unit'

units_added_count = 0

not_added = []

for unit in data['champions']:
    post_response = requests.post(base_url, json=unit, verify=False)
    if post_response.status_code == 201:
        units_added_count += 1    
    else:
      error_message = post_response.content.decode('utf-8')
      not_added.append(unit['name'])
      print(f"Error adding trait {unit['name']}: Unknown error: {error_message}")


print(f"Items added successfully: {units_added_count}")

if not_added:
    print("Items not added successfully:")
    for unit_name in not_added:
        print(unit_name)
else:
    print("All items added successfully.")
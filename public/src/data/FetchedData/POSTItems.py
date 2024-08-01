import requests
import json

with open('items.json', 'r') as file:
    data = json.load(file)

base_url = 'https://localhost:7235/api/item'

items_added_count = 0

not_added = []

for item in data['items']:
    post_response = requests.post(base_url, json=item, verify=False)
    if post_response.status_code == 201:
        items_added_count += 1
    else:
        not_added.append(item['name'])


print(f"Items added successfully: {items_added_count}")

if not_added:
    print("Items not added successfully:")
    for item_name in not_added:
        print(item_name)
else:
    print("All items added successfully.")
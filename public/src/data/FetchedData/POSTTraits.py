import requests
import json

with open('traits.json', 'r') as file:
    data = json.load(file)

base_url = 'https://localhost:7235/api/trait'

traits_added_count = 0

not_added = []

for item in data["traits"]:
    post_response = requests.post(base_url, json=item, verify=False)
    if post_response.status_code == 201:
        traits_added_count += 1
    else:
        error_message = post_response.content.decode('utf-8')
        not_added.append(item['key'])
        print(f"Error adding trait {item['key']}: Unknown error: {error_message}")


print(f"Traits added successfully: {traits_added_count}")

if not_added:
    print("Traits not added successfully:")
    for trait_name in not_added:
        print(trait_name)
else:
    print("All traits added successfully.")
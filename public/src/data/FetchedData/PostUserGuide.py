import requests
import json

with open('ab.json', 'r') as file:
    data = json.load(file)

base_url = 'https://localhost:7235/api/userguide/1'

auth_token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6Ik5vYWgiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJub2FoY3Jhc3NAaG90bWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiTm9haCIsIlVzZXJJZCI6IjEiLCJleHAiOjE3MTc2NTM3MTUsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzIzLyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzIzLyJ9.ZqeoF0L3r6VWmW7G7zNlDKmFWNfRdW4szJnPR7RKgnw'

headers = {'Authorization': 'Bearer ' + auth_token}

post_response = requests.post(base_url, json=data, headers=headers, verify=False)

if post_response.status_code == 200:
    print("Request was successful.")
else:
    print(f"Request failed with status code: {post_response.status_code}")
    print(post_response.content.decode('utf-8'))
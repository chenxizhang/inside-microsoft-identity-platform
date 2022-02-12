# pip install msal

from msal import ConfidentialClientApplication

client_id = "85e5dba4-9cad-4191-87e2-c75ab83c957e"
authority = "https://login.microsoftonline.com/3a6831ab-6304-4c72-8d08-3afe544555dd"
client_secret = "MtQ7Q~hJpoaXFzyZBoBUnvhK7X~U-mLRngZ7N"

app = ConfidentialClientApplication(client_id, client_secret, authority)
token = app.acquire_token_for_client("https://graph.microsoft.com/.default")

# 安装sdk
# pip install msal

from msal import ConfidentialClientApplication

client_id = "85e5dba4-9cad-4191-87e2-c75ab83c957e"
authority = "https://login.microsoftonline.com/3a6831ab-6304-4c72-8d08-3afe544555dd"
client_secret = "MtQ7Q~hJpoaXFzyZBoBUnvhK7X~U-mLRngZ7N"


# 直接用密钥来获取凭据
app = ConfidentialClientApplication(client_id, client_secret, authority)
token = app.acquire_token_for_client("https://graph.microsoft.com/.default")

print(token)

# 用证书来获取凭据 
# 这个可以通过openssl来生成 
# openssl pkcs12 -in identityplatform.pfx -out out.pem -nodes
key ="""-----BEGIN PRIVATE KEY-----
MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDAVZpqBeRFQCfq
PQqctj4twr+VgchsNE3LUEZn4hTO/ClYJLa4AlEfLIiXHjUoT5WIyWoO+bqbJGL6
pkdb4XuC5xKDPMd82e4TeUlqOIZfDXgmaYvbLLVme81LazzOB29sCkoyz5OSFzKE
fxECXfrIHV8e+Cf4qjiav9zoMls2CpyNSau6gunOAK9sImociltChHKFg7NEM0EN
a1kEW1fT1U5PaIQxzlODIaZjise6yr1U+xqaSD0mZ6Fe9WSZ3qi0xhBLNoISMuLM
tSIeUHH3TfPYTjzEau9OUcSCqX8Zf3rfqrTk0hD+nlPCo1VabaRI+jwN6R55TlMk
/hYExH/5AgMBAAECggEBAKqEemUbGhlHWtvyCFGNxSye52nuaTl9WacyYWlCaD5m
E2WhDxmufCtOOT75OhmmDSX7o/ro7scTCGm+N6+/Bdi1cpVFsnr/X3KGak8xE/h7
oZU0qBjF14Gnqwf9aCglMWSw7r2DiQrRZClR2kul9GuslqOHUTDGAW85QOnfkzwh
vJNJU5pQus+zwmmd1hivtRXtbMXmv91vxsxDoWfKPRoOG1yfdTuXD+ty6ipcmeZz
fwkGvo5vk0viIGFRin/dU+tdDZaLr6jBgqt+Q4wlAPK9zQrW7g6/Co52gGEWF59C
CvBS2lUQtv2IpEs/My7odVACS6jHi2FXbPNSa0kZPj0CgYEA5m8iPKD79KKmeBLh
q88cJhXlVd8bGj4IHh+n2jPUR/pD7D50qjW88PryKFvE704Hxxw3qp0Jz2uY3PHm
ROhV3XKSXcTxkC05LN0kmJL2i5zdBv8Sg5+5Ho9Hr90u98CCcSiiFDhgJUbIS+zc
+m4yJ+wvUKbH0cN73aX5hnBmBPMCgYEA1axZKiXbpy+q8R2oG9Ec0Bb7SB0M2nFu
8pqaf9E4jEuQB1Jz1RJfwGl/qR14BqpOim6LLNHHRTge0XPXGAndsGs2e237fxqR
oqshomxMxQX0D5z9mFjDjpIeyhYEqVjQTIMi/7Xxri7p089To7RaYSUN8dFPAaey
inTbJF8KkmMCgYEAnUrY5OfCdH+eADJrRrqt7TVfARm0x1n2cpGLIv/j9GnZeWY8
fn8GBLxXFcNmjy8FUh1pxhVBwAsjIhYg54JsMiflzwoDFjY4Y+5j753Jmw0tNnxg
Z+ZcF4cYGOxzBfyrTZC96e19e5RwXptFT9Bufh/TQEtH9GBqqaaEyAbrLI8CgYAN
IEtR9YVq3djIeyPqoYv9lIvXQEGb4cAJE0pOc9HffHzalkwbWMedEF4RS4gmEKxG
gMf39uMg83OhNlaOWXzO66crKfR7OGyd65ljWvfUWqtFkkVZ6IoK5hsSGwwqQxR4
vs2Vm92+747ZvjDLK7cppJcYtdz+owiqzMbxkw0ZNQKBgQCqQtavabv7zAy+dc4z
g4J3hBy6Zlm5XRGDekSIOrah8hqsHoJqcnQcMMYDaGE/UalLmExSHd4Wbxqj4wwP
I2DNxc5fEumiAKHfq4RN9qnBLzJz3ZQATwHwBef4n5cOJ6T9MYtQZIW7APTgEspM
VhQM9/W8BtBu/w1my+jfdMUSyQ==
-----END PRIVATE KEY-----"""

client_certificate ={
    "private_key":key,
    "thumbprint":"A639157B5BBC31DC007CC014B077F8D70A082122"
}

app = ConfidentialClientApplication(client_id,client_certificate,authority)
token = app.acquire_token_for_client("https://graph.microsoft.com/.default")
print(token)


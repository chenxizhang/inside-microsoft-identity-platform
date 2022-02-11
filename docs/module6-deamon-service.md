

## 创建证书并用于应用程序

```powershell 
# 用管理员身份打开PowerShell

$cert = New-SelfSignedCertificate -CertStoreLocation Cert:\LocalMachine\My -DnsName "identityplatform.xizhang.com" -KeyExportPolicy Exportable -Provider "Microsoft Enhanced RSA and AES Cryptographic Provider"

# 导出证书公钥（用于上传到Azure）
Export-Certificate -Cert $cert -FilePath C:\temp\identityplatform.cer

# 导出证书私钥（用于在其他电脑上安装）
$pwd = ConvertTo-SecureString -String "xxxx" -AsPlainText -Force
Export-PfxCertificate -Cert $cert -FilePath c:\temp\identityplatform.pfx -Password $pwd

```
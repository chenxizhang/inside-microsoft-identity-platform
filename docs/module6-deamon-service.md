---
marp: true
headingDivider: 3
paginate: true
title: 第六讲：为守护程序或后端服务集成 
style: |
    section footer{
        margin-left:50px
    }
    
    li>strong{
        font-size:32px;
        padding:5px;
        background-color:#CCCCCC
    }
---

# 第六讲：为守护程序或后端服务集成 
> **解密和实战 Microsoft Identity Platform**  https://identityplatform.xizhang.com
![bg fit left:30% opacity:0.2](images/aad.png)


作者：陈希章
时间：2022年2月


## 课程大纲
<!--
footer: '**解密和实战 Microsoft Identity Platform**  https://identityplatform.xizhang.com'
-->

1. [基本概念](module1-overview.md)
1. [为单页应用程序集成 （`React`）](module2-spa.md)
1. [为Web应用程序集成 （`Node.js`）](module3-webapp.md)
1. [使用Microsoft Identity 保护Web API （`ASP.NET Core`）](module4-webapi.md)
1. [为移动或桌面应用程序集成 （`Xamarin, WPF`）](module5-desktop-mobile.md)
1. [为守护程序或后端服务集成 (`Azure function + Python，Power Automate`)](module6-deamon-service.md)
1. [Azure AD B2C应用集成 (`React，手机验证码登录和微信登录`） ](module7-b2c.md)
1. [使用 Microsoft Graph API (`Graph explorer & Postman`)](module8-msgraph.md)
1. [使用 Azure AD PowerShell 模块 (`PowerShell`)](module9-powershell.md)
1. [应用管理及最佳实践](module10-bestpractices.md)

## 为守护程序或后端服务集成 

不代表用户身份，经常用于自动化执行任务，或者是后台服务

1. 注册应用程序
    - 注意申请应用程序权限，而不是委托权限
    - 需要管理员授权
1. 创建密钥
    - 或创建并上传证书
1. 使用SDK编程调用
    - Client Credential

## 注册应用程序
<!-- _footer: '' -->
![](images/aad-deamon-app.png)


## 创建密钥
<!-- MtQ7Q~hJpoaXFzyZBoBUnvhK7X~U-mLRngZ7N -->
<!-- _footer: '' -->

![](images/aad-app-client-secret.png)

## 创建证书并用于应用程序 【更安全】
<!-- _footer: '' -->

```powershell 
# 用管理员身份打开PowerShell
$cert = New-SelfSignedCertificate `
    -CertStoreLocation Cert:\LocalMachine\My `
    -DnsName "identityplatform.xizhang.com" `
    -KeyExportPolicy Exportable `
    -Provider "Microsoft Enhanced RSA and AES Cryptographic Provider"
# 导出证书公钥（用于上传到Azure）
Export-Certificate -Cert $cert -FilePath C:\temp\identityplatform.cer
# 导出证书私钥（用于在其他电脑上安装）
$pwd = ConvertTo-SecureString -String "xxxx" -AsPlainText -Force
Export-PfxCertificate -Cert $cert -FilePath c:\temp\identityplatform.pfx -Password $pwd
```

![bg right fit](images/aad-app-certification.png)

## 使用 PowerShell Msal SDK

```powershell
Install-Module -Name MSAL.PS -Scope CurrentUser
# MSAL.PS 不是官方库，但也是由微软员工编写的

$token= Get-MsalToken `
    -ClientId 85e5dba4-9cad-4191-87e2-c75ab83c957e `
    -TenantId 3a6831ab-6304-4c72-8d08-3afe544555dd `
    -ClientSecret (ConvertTo-SecureString `
        "MtQ7Q~hJpoaXFzyZBoBUnvhK7X~U-mLRngZ7N" `
        -Force -AsPlainText)
# 或用证书
$token= Get-MsalToken `
    -ClientId 85e5dba4-9cad-4191-87e2-c75ab83c957e `
    -TenantId 3a6831ab-6304-4c72-8d08-3afe544555dd `
    -ClientCertificate `
        (Get-ChildItem `
            "Cert:\LocalMachine\My\A639157B5BBC31DC007CC014B077F8D70A082122")
```
![bg right fit](images/powershell-token-app.png)


## 使用MSAL for python 编程

```python
# 安装sdk
# pip install msal

from msal import ConfidentialClientApplication

client_id = "85e5dba4-9cad-4191-87e2-c75ab83c957e"
authority = "https://login.microsoftonline.com/3a6831ab-6304-4c72-8d08-3afe544555dd"
client_secret = "MtQ7Q~hJpoaXFzyZBoBUnvhK7X~U-mLRngZ7N"

app = ConfidentialClientApplication(client_id, client_secret, authority)
token = app.acquire_token_for_client("https://graph.microsoft.com/.default")
```

## 使用MSAL for python 编程(证书)
1. 导出私钥 (Linux自带该工具，可以用`WSL`)
    `openssl pkcs12 -in identityplatform.pfx -out out.pem -nodes`
1. 编程访问
    ``` python
    key ="""-----BEGIN PRIVATE KEY-----
    MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDAVZpqBeRFQCfq
    VhQM9/W8BtBu/w1my+jfdMUSyQ==
    -----END PRIVATE KEY-----"""

    client_certificate ={
        "private_key":key,
        "thumbprint":"A639157B5BBC31DC007CC014B077F8D70A082122"
    }

    app = ConfidentialClientApplication(client_id,client_certificate,authority)
    token = app.acquire_token_for_client("https://graph.microsoft.com/.default")
    ```

## 在Power Automate中使用

1. 默认支持委托的权限
1. 应用权限通过配置实现

![bg right:70% fit](images/powerautomate.png)

## 课程反馈

你可以通过邮件 <ares@xizhang.com> 与我取得联系，也可以关注 `code365xyz` 这个微信公众号给我留言，还可以在这里 (<https://github.com/chenxizhang/inside-microsoft-identity-platform/discussions>) 给我提出问题或讨论。

![](images/code365xyz.jpg)


陈希章 于上海
2022年2月
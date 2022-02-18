---
marp: true
headingDivider: 3
paginate: true
title: 第十讲：应用管理及最佳实践
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

# 第十讲：应用管理及最佳实践
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
1. [为守护程序或后端服务集成 (`Azure function +Python，Power Automate`)](module6-deamon-service.md)
1. [Azure AD B2C应用集成 (`React，手机验证码登录和微信登录`） ](module7-b2c.md)
1. [使用 Microsoft Graph API (`Graph explorer & Postman`)](module8-msgraph.md)
1. [使用 Azure AD PowerShell 模块 (`PowerShell`)](module9-powershell.md)
1. **[应用管理及最佳实践](module10-bestpractices.md)**


##  应用管理及最佳实践

1. 多租户应用
1. 国内版应用
1. 自定义令牌设置

## 多租户应用
1. 注册应用程序
1. 测试应用程序

![bg right:60% fit](images/multi-tenant-app.png)

## 测试多租户应用

请参考第五讲中最后PowerShell的部分。

```powershell
# 安装PowerShell模块
Install-Module MSAL.PS -Scope CurrentUser

# 仅作身份验证, 返回值中IdToken包含了用户基本信息，可以直接解码
$token = Get-MsalToken `
    -ClientId 14308ef0-3c9c-4100-96ba-5d2d805cac0f

# 获取个人基本信息

curl -Uri "https://graph.microsoft.com/v1.0/me" `
    -Headers @{"Authorization"="Bearer $($token.AccessToken)"}

```

## 获取其他资源

1. Microsoft 个人账号能成功授权
1. Microsoft 365 账号可能无法成功授权

```powershell
# 身份验证并且获取其他资源权限
$token = Get-MsalToken `
    -ClientId 14308ef0-3c9c-4100-96ba-5d2d805cac0f `
    -Scopes "Mail.Read Files.Read"

# 获取个人邮件
curl -Uri "https://graph.microsoft.com/v1.0/me/messages" `
    -Headers @{"Authorization"="Bearer $($token.AccessToken)"}

# 获取个人文件
curl -Uri "https://graph.microsoft.com/v1.0/me/drive/root" `
    -Headers @{"Authorization"="Bearer $($token.AccessToken)"}
```

![bg right fit](images/unverified-app-error.png)

## 多租户应用设置（推荐完成发布者验证）
<!-- _footer: '' -->
<!-- https://docs.microsoft.com/zh-cn/azure/active-directory/develop/publisher-verification-overview -->
![](images/verify-publisher.png)

## 租户设置 （如果下面这样设置是可以的，但是存在一定的风险）
<!-- https://portal.azure.com/#blade/Microsoft_AAD_IAM/ConsentPoliciesMenuBlade/UserSettings -->

![](images/azuread-user-consent-settings.png)

## 请求管理员同意

`https://login.microsoftonline.com/ab7d3ddf-d9bf-465f-83dc-49833f69440f/v2.0/adminconsent?client_id=14308ef0-3c9c-4100-96ba-5d2d805cac0f&state=12345&redirect_uri=http://localhost&scope=Mail.Read Files.Read`


![bg fit right](images/admin-consent-screen.png)


## 国内版应用开发

1. 门户 https://portal.azure.cn/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade
1. 登录（Authority） https://login.partner.microsoftonline.cn
1. Microsoft Graph 端点地址 https://microsoftgraph.chinacloudapi.cn

![bg fit right](images/azure-china-endpoint.png)

## 国内版应用开发（使用PowerShell）

```powershell
# 关于使用PowerShell,请参考第五讲结尾部分
Install-Module MSAL.PS -Scope CurrentUser

$app = New-MsalClientApplication `
    -ClientId 2b183a93-03a2-46a3-bc06-4711e57d2caa `
    -AzureCloudInstance AzureChina `
    -Authority https://login.partner.microsoftonline.cn/2dce9a6e-6fe1-4dc8-ac10-f571cdefc583


Get-MsalToken `
    -PublicClientApplication $app `
    -Scopes "https://microsoftgraph.chinacloudapi.cn/.default"

```

![bg right fit](images/aad-msaltoken-china.png)

## 自定义令牌

1. 配置令牌属性
1. 配置令牌生命有效期

###  自定义令牌属性
<!-- https://docs.microsoft.com/zh-cn/azure/active-directory/develop/id-tokens -->
<!-- _footer: ''--> 
![bg fit](images/custom-token-settings.png)
![bg fit](images/custom-token-group-claims.png)


### 配置令牌生命周期

1. 使用此功能需要 Azure AD Premium P1 许可证
1. 默认是两小时。

```powershell
Install-Module AzureADPreview

Connect-AzureAD -Confirm
# 创建一个策略
$policy = New-AzureADPolicy `
    -Definition @('{"TokenLifetimePolicy":{"Version":1,"AccessTokenLifetime":"02:00:00"}}') `
    -DisplayName "WebPolicyScenario" `
    -IsOrganizationDefault $false -Type "TokenLifetimePolicy"

# 获取某个企业应用程序的引用
$sp = Get-AzureADServicePrincipal `
    -Filter "DisplayName eq '<service principal display name>'"

# 为该应用程序指派该策略
Add-AzureADServicePrincipalPolicy `
    -Id $sp.ObjectId `
    -RefObjectId $policy.Id

```

## 课程反馈

你可以通过邮件 <ares@xizhang.com> 与我取得联系，也可以关注 `code365xyz` 这个微信公众号给我留言，还可以在这里 (<https://github.com/chenxizhang/inside-microsoft-identity-platform/discussions>) 给我提出问题或讨论。

![](images/code365xyz.jpg)


陈希章 于上海
2022年2月
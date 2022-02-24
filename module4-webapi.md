---
marp: true
headingDivider: 3
paginate: true
title: 第四讲：使用Microsoft Identity 保护Web API
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

# 第四讲：使用Microsoft Identity 保护Web API
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
1. **[使用Microsoft Identity 保护Web API （`ASP.NET Core`）](module4-webapi.md)**
1. [为移动或桌面应用程序集成 （`Xamarin, WPF`）](module5-desktop-mobile.md)
1. [为守护程序或后端服务集成 (`Azure function +Python，Power Automate`)](module6-deamon-service.md)
1. [Azure AD B2C应用集成 (`React，手机验证码登录和微信登录`） ](module7-b2c.md)
1. [使用 Microsoft Graph API (`Graph explorer & Postman`)](module8-msgraph.md)
1. [使用 Azure AD PowerShell 模块 (`PowerShell`)](module9-powershell.md)
1. [应用管理及最佳实践](module10-bestpractices.md)


## <!-- fit --> 使用Microsoft Identity 保护Web API

1. 注册应用程序
1. 使用ASP.NET创建Web API 项目
1. 在桌面客户端中调用Web API

## 注册应用程序

1. 不需要回调地址
1. 公开API

![bg right:60% fit](images/expose-api.png)


## 使用ASP.NET创建Web API 项目
<!-- _footer: '' -->
```powershell
dotnet new webapi -o 项目名称 -au singleorg --client-id 应用程序编号 --tenant-id 租户编号 --call-graph $true --called-api-scopes "mail.read user.read"
```

默认生成的配置文件

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "qualified.domain.name",
    "TenantId": "3a6831ab-6304-4c72-8d08-3afe544555dd",
    "ClientId": "d4755e4f-9ff6-463a-afed-ebd9c40d73d7",
    "ClientSecret": "3pT7Q~lj9QDuDc~JaTTNHsu02ZLY5WtaxWUze",
    "ClientCertificates": [],
    "Scopes": "access_as_user",
    "CallbackPath": "/signin-oidc"
  },
  "DownstreamApi": {
    "BaseUrl": "https://graph.microsoft.com/v1.0",
    "Scopes": "mail.read user.read"
  }
}
```

### 依赖注入控制器

```csharp

private readonly IDownstreamWebApi _downstreamWebApi;
private readonly ILogger<string> _logger;
private readonly GraphServiceClient _client;

public TestController(
    IDownstreamWebApi downstreamWebApi, 
    ILogger<string> logger, 
    GraphServiceClient client)
{
    _downstreamWebApi = downstreamWebApi;
    _client = client;
    _logger = logger;
}
```

### 检查调用者的身份
<!-- https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-protected-web-api-verification-scope-app-roles?tabs=aspnetcore -->

1. 声明式
    ```csharp
    [Authorize]//要求身份认证
    [Authorize(Roles = "access_as_application")]//是否要求指定角色访问，包括AAD内置角色，以及应用程序角色
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]// 读取配置文件
    [RequiredScope(scopeRequiredByApi)]//直接用文本表达
    ```
1. 代码检查
    ```csharp
    HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
    HttpContext.ValidateAppRole("access_as_application");//验证用户或应用程序是否属于某个应用程序角色
    ```



### 调用下游API （ On-behalf-of 流程）
使用 `DownstreamWebApi` 帮助类

```csharp
using var response = await _downstreamWebApi.CallWebApiForUserAsync("DownstreamApi", op => op.RelativePath = "/me/messages?$select=subject").ConfigureAwait(false);
if (response.StatusCode == System.Net.HttpStatusCode.OK)
{
    var apiResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    // Do something
    return apiResult;
}
else
{
    var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}: {error}");
}
```

使用 `GraphServiceClient`

```csharp
var result = await _client.Me.Messages.Request().Select(x => x.Subject).GetAsync().ConfigureAwait(false);
return result.Select(x => x.Subject).ToArray();
```

## 在桌面客户端中调用Web API
<!-- _footer: '' -->
1. 注册应用程序（最好是单独注册一个）
1. 使用 `Microsoft.Identity.Client` 登录并获取凭据

    ```csharp
        using Microsoft.Identity.Client;
        var scopes = new[] { "api://d4755e4f-9ff6-463a-afed-ebd9c40d73d7/access_as_user", "mail.read", "user.read" };
        var tenantId = "3a6831ab-6304-4c72-8d08-3afe544555dd";
        var clientId = "1c6b9008-113d-4854-afd3-e3f5bd726ce7";

        var pca = PublicClientApplicationBuilder
            .Create(clientId)
            .WithTenantId(tenantId)
            .WithRedirectUri("http://localhost")
            .Build();
        var result = await pca.AcquireTokenInteractive(scopes)
            .ExecuteAsync();
    ```
1. 使用凭据访问Webapi
    ```csharp
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
        var temp = await client.GetAsync("https://localhost:7032/test");
        Console.WriteLine(await temp.Content.ReadAsStringAsync());
    ```


## 思考题

1. 如果有多个Webapi应该怎么办
1. Microsoft Graph其实也是一系列API，到底怎么做出来的

## 课程反馈

你可以通过邮件 <ares@xizhang.com> 与我取得联系，也可以关注 `code365xyz` 这个微信公众号给我留言。

#### 视频教程

[点击这里](https://study.163.com/course/introduction.htm?courseId=1212500806&share=2&shareId=400000000620030) 或扫码可以访问配套视频教程。

![bg right:30% fit](images/videocourse.png)


陈希章 2022年2月 于上海
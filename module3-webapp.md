---
marp: true
headingDivider: 3
paginate: true
title: 第三讲：为Web应用程序集成
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

# 第三讲：为Web应用程序集成
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
1. **[为Web应用程序集成 （`Node.js`）](module3-webapp.md)**
1. [使用Microsoft Identity 保护Web API （`ASP.NET Core`）](module4-webapi.md)
1. [为移动或桌面应用程序集成 （`Xamarin, WPF`）](module5-desktop-mobile.md)
1. [为守护程序或后端服务集成 (`Azure function +Python，Power Automate`)](module6-deamon-service.md)
1. [Azure AD B2C应用集成 (`React，手机验证码登录和微信登录`） ](module7-b2c.md)
1. [使用 Microsoft Graph API (`Graph explorer & Postman`)](module8-msgraph.md)
1. [使用 Azure AD PowerShell 模块 (`PowerShell`)](module9-powershell.md)
1. [应用管理及最佳实践](module10-bestpractices.md)


##  为 Web 应用程序集成

这里的Web应用程序是指区分于以React等纯前端框架开发的静态网站，需要一个服务器处理请求的应用。

1. 注册应用程序
1. Nodejs + express + MSAL-node
1. ASP.NET Core + Microsoft.Identity.Web
1. 使用Azure app service 进行身份验证

## 注册应用程序
<!-- _footer: '' -->
![bg fit right](images/aad-app-webapp.png)
1. 填写一个或多个回调地址
1. `ASP.NET`中间件，signin-oidc
1. `ASP.NET` 勾选 id_token 选项

## Nodejs + express + MSAL-node

最小化项目演示

1. 安装 `@azure/msal-node` 和 `express`
1. 初始化应用程序以及缓存
1. 构建身份验证地址并跳转
1. 接受回调并读取用户身份


### 初始化应用程序以及缓存

```javascript
const config = {
    auth: {
        clientId: "b342d605-dbf8-4fd8-ac21-daddc664984c",
        authority: "https://login.microsoftonline.com/3a6831ab-6304-4c72-8d08-3afe544555dd",
        clientSecret: "DVf7Q~SGK9Iywggz9mrdVzfYy5wHyXZrb4dTe"
    },
    cache: {
        plugin
    }
}

const cca = new msal.ConfidentialClientApplication(config);
const tokenCache = cca.getTokenCache();

```

### 构建身份验证地址并跳转

```javascript
app.get("/", async (req, res) => {

    const accounts = await tokenCache.getAllAccounts();

    if (accounts && accounts.length > 0) {
        res.status(200).send(accounts[0]);
    }
    else {
        const authCodeUrlParameters = {
            scopes: ['user.read'],
            redirectUri: "http://localhost:3000/redirect"
        };

        cca.getAuthCodeUrl(authCodeUrlParameters)
            .then(response => res.redirect(response))
            .catch((error) => console.log(JSON.stringify(error)));
    }
});
```



### 接受回调并读取用户身份

```javascript
app.get('/redirect', (req, res) => {
    const tokenRequest = {
        code: req.query.code,
        scopes: ['user.read'],
        redirectUri: 'http://localhost:3000/redirect'
    };

    cca.acquireTokenByCode(tokenRequest).then((response) => {
        res.redirect('/');
    }).catch((error) => {
        res.status(500).send(error);
    })
})

```

![bg right fit](images/node-express-account.png)


## ASP.NET Core + Microsoft.Identity.Web
1. 创建项目
1. 访问当前用户身份
1. 运行项目

### 创建 ASP.NET Core项目

```powershell
dotnet new webapp -o 项目名称 -au singleorg --client-id 应用程序编号 --tenant-id 租户编号
```

可用的身份认证选项如下

![](images/dotnet-webapp-au-options.png)


### 控制页面是否需要（或不需要）身份验证

修改根目录下面的 `program.cs`文件， 这是可选的，默认情况下，如果不加下面的设置，则所有页面都需要身份验证。

```csharp
builder.services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Contact");//这个页面需要身份验证
    options.Conventions.AuthorizeFolder("/Private");//这个目录需要身份验证
    options.Conventions.AllowAnonymousToPage("/Private/PublicPage");//这个页面可匿名访问
    options.Conventions.AllowAnonymousToFolder("/Private/PublicPages");//这个目录可匿名访问
});
```

### 访问当前用户身份

修改 `pages/index.cshtml`的代码如下，直接用 `User` 对象。在 `.cs` 文件中，也是用 `User` 对象。

```csharp
@page
@model IndexModel
@{
    ViewData["Title"] = "Home page";
}

<div class="text-center">
    <h1 class="display-4">用户信息</h1>
    <p>@User.Identity.Name</p>
</div>

```

### 本地运行和调试 ASP.NET Core 项目

`dotnet run --urls https://localhost:3000`

默认运行 `dotnet run` 每次的端口可能不一样，所以上面指定了固定的端口，这个需要跟在AAD中注册应用程序时填写的重定向 Url 匹配。请注意，.NET Core要求用https，否则会报一个错误。
> `/signin-oidc` 这个地址是 `ASP.NET Core`中间件自动监听的。在网站中找不到。

![](images/webapp.png)


### 在 ASP.NET Core 项目中访问Microsoft Graph
<!-- https://docs.microsoft.com/en-us/samples/azure-samples/active-directory-aspnetcore-webapp-openidconnect-v2/active-directory-aspnetcore-webapp-openidconnect-v2/ -->

通过下面的命令可以创建项目

```powershell
dotnet new webapp -o 项目名称 -au singleorg --client-id 应用程序编号 --tenant-id 租户编号 --calls-graph $true --called-api-scopes "mail.read user.read"
```

配置有关中间件
```csharp

services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph(Configuration.GetSection("DownstreamApi"))
    .AddInMemoryTokenCaches();

```

### 注入到控制器中
```csharp
private readonly ILogger<HomeController> _logger;
  private readonly GraphServiceClient _graphServiceClient;
 
  private readonly GraphServiceClient _graphServiceClient;
  public HomeController(ILogger<HomeController> logger,
                     IConfiguration configuration,
                     GraphServiceClient graphServiceClient)
  {
   _logger = logger;
   _graphServiceClient = graphServiceClient;
   this._consentHandler = consentHandler;
  }
```

### 在具体的方法中调用

```csharp
[AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
public async Task<IActionResult> Profile()
{
 var me = await _graphServiceClient.Me.Request().GetAsync();
 ViewData["Me"] = me;

 try
 {
     // Get user photo
     using (var photoStream = await _graphServiceClient.Me.Photo.Content.Request().GetAsync())
     {
         byte[] photoByte = ((MemoryStream)photoStream).ToArray();
         ViewData["Photo"] = Convert.ToBase64String(photoByte);
     }
 }
 catch (System.Exception)
 {
     ViewData["Photo"] = null;
 }

 return View();
}
```


## 使用Azure app service 进行身份验证
<!-- 如果需要顺便读取Microsoft Graph，则需要按照这里进行配置 https://docs.microsoft.com/zh-cn/azure/app-service/scenario-secure-app-access-microsoft-graph-as-user?tabs=azure-resource-explorer%2Cprogramming-language-nodejs#configure-app-service-to-return-a-usable-access-token -->

<!-- _footer: '' -->

无需在代码里面实现身份验证和授权，服务器将发送如下的header信息

1. x-ms-client-principal-name
1. x-ms-client-principal-id
1. x-ms-client-principal-idp
1. x-ms-client-principal
1. x-ms-token-aad-access-token
1. x-ms-token-aad-id-token

![bg right fit](images/node-express-web-easy-auth.png)


### 用户进行授权
<!-- 
    "login": {
        "loginParameters": [
        "response_type=code id_token",
        "scope=openid offline_access profile https://graph.microsoft.com/Mail.Read"
        ],
        "disableWWWAuthenticate": false
    },

 -->
1. 默认只读取基本profile
1. 可以通过配置实现对Microsoft Graph的访问授权
1. 代码中可以通过 `x-ms-token-aad-access-token` 进行后续调用

![bg left fit](images/easy-auth-msgraph-settings.png)
![bg fit](images/node-express-web-easy-auth-consent.png)


## 课程反馈

你可以通过邮件 <ares@xizhang.com> 与我取得联系，也可以关注 `code365xyz` 这个微信公众号给我留言。

#### 视频教程

[点击这里](https://study.163.com/course/introduction.htm?courseId=1212500806&share=2&shareId=400000000620030) 或扫码可以访问配套视频教程。

![bg right:30% fit](images/videocourse.png)


陈希章 2022年2月 于上海
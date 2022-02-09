using System.Security;
using Microsoft.Graph;
using Microsoft.Identity.Client;

var scopes = new[] { "User.Read" };

// Multi-tenant apps can use "common",
// single-tenant apps must use the tenant ID from the Azure portal
var tenantId = "3a6831ab-6304-4c72-8d08-3afe544555dd";

// Value from app registration
var clientId = "87700721-9a44-4470-9099-d079aab1c3d6";

var pca = PublicClientApplicationBuilder
    .Create(clientId)
    .WithTenantId(tenantId)
    .WithRedirectUri("http://localhost")
    .Build();

#region 使用code grant机制
var authProvider = new DelegateAuthenticationProvider(async (request) =>
{
    // Use Microsoft.Identity.Client to retrieve token
    var result = await pca.AcquireTokenInteractive(scopes)
        .ExecuteAsync();
    request.Headers.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
});

var graphClient = new GraphServiceClient(authProvider);

var me = await graphClient.Me.Request().GetAsync();

Console.WriteLine(me.DisplayName);

#endregion


#region 使用Windows集成验证
// var authProvider = new DelegateAuthenticationProvider(async (request) =>
// {
//     // Use Microsoft.Identity.Client to retrieve token
//     var result = await pca.AcquireTokenByIntegratedWindowsAuth(scopes).ExecuteAsync();
//     System.Console.WriteLine(result.AccessToken);
//     request.Headers.Authorization =
//         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
// });
// var graphClient = new GraphServiceClient(authProvider);

// var me = await graphClient.Me.Request().GetAsync();

// Console.WriteLine(me.DisplayName);
#endregion

#region 使用用户名和密码
// Console.WriteLine("请输入用户名");
// var username = Console.ReadLine();
// SecureString securePwd = new SecureString();
// ConsoleKeyInfo key;

// Console.WriteLine("请输入密码");
// do
// {
//     key = Console.ReadKey(true);
//     securePwd.AppendChar(key.KeyChar);
//     Console.Write("*");
// } while (key.Key != ConsoleKey.Enter);


// var authProvider = new DelegateAuthenticationProvider(async (request) =>
// {
//     // Use Microsoft.Identity.Client to retrieve token
//     var result = await pca.AcquireTokenByUsernamePassword(scopes, username, securePwd).ExecuteAsync();
//     request.Headers.Authorization =
//         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
// });

// var graphClient = new GraphServiceClient(authProvider);

// var me = await graphClient.Me.Request().GetAsync();

// Console.WriteLine(me.DisplayName);

#endregion


#region 使用设备代码


// var authProvider = new DelegateAuthenticationProvider(async (request) =>
// {
//     // Use Microsoft.Identity.Client to retrieve token
//     var result = await pca.AcquireTokenWithDeviceCode(scopes, r =>
//     {
//         Console.WriteLine(r.UserCode);
//         return Task.FromResult(0);
//     }).ExecuteAsync();



//     request.Headers.Authorization =
//         new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
// });

// var graphClient = new GraphServiceClient(authProvider);

// var me = await graphClient.Me.Request().GetAsync();

// Console.WriteLine(me.DisplayName);


#endregion
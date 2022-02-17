using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Security;

var scopes = new[] { "User.Read" };
var tenantId = "3a6831ab-6304-4c72-8d08-3afe544555dd";
var clientId = "87700721-9a44-4470-9099-d079aab1c3d6";

var pca = PublicClientApplicationBuilder
    .Create(clientId)
    .WithTenantId(tenantId)
    .WithRedirectUri("http://localhost")
    .Build();

Console.WriteLine("请输入数字,告诉我们你要选择的类型:");
Console.WriteLine("\t1.交互式访问");
Console.WriteLine("\t2.Windows集成身份");
Console.WriteLine("\t3.用户名和密码");
Console.WriteLine("\t4.设备代码");

var input = Console.ReadLine();
var kind = 0;

if (int.TryParse(input, out kind))
{
    var authProvider = await createAuthProvider(kind);
    var graphClient = new GraphServiceClient(authProvider);
    var me = await graphClient.Me.Request().GetAsync();
    Console.WriteLine(me.DisplayName);
}

/// <summary>
/// 这个方法来创建不同的authProvder，并且包装一个DelegateAuthenticationProvider
/// </summary>
async Task<DelegateAuthenticationProvider> createAuthProvider(int kind)
{
    Func<Task<AuthenticationResult>> getUserPasswordCredential = async () =>
    {
        Console.WriteLine("请输入用户名");
        var username = Console.ReadLine();
        SecureString securePwd = new SecureString();
        ConsoleKeyInfo key;

        Console.WriteLine("请输入密码");
        do
        {
            key = Console.ReadKey(true);
            securePwd.AppendChar(key.KeyChar);
            Console.Write("*");
        } while (key.Key != ConsoleKey.Enter);

        return await pca.AcquireTokenByUsernamePassword(scopes, username, securePwd).ExecuteAsync();
    };

    var result = kind switch
    {
        1 => await pca.AcquireTokenInteractive(scopes).ExecuteAsync(),
        2 => await pca.AcquireTokenByIntegratedWindowsAuth(scopes).ExecuteAsync(),
        3 => await getUserPasswordCredential(),
        4 => await pca.AcquireTokenWithDeviceCode(scopes, (v) =>
        {
            Console.WriteLine(v.Message);
            return Task.FromResult(0);
        }).ExecuteAsync(),
        _ => await pca.AcquireTokenInteractive(scopes).ExecuteAsync()
    };

    return new DelegateAuthenticationProvider(request =>
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
        return Task.FromResult(0);
    });
}
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


var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);

var temp = await client.GetAsync("https://localhost:7032/test");

Console.WriteLine(await temp.Content.ReadAsStringAsync());
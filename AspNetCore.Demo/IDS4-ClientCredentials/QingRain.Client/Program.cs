// See https://aka.ms/new-console-template for more information
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

Console.WriteLine("Hello, World!");

await Task.Delay(TimeSpan.FromSeconds(10));

var client = new HttpClient();

var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}

var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
{
    Address = disco.TokenEndpoint,
    ClientId = "client",
    ClientSecret = "secret",

    Scope = "api1"
});
if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    return;
}
Console.WriteLine(tokenResponse.Json);

// call api
var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken);
var response = await apiClient.GetAsync("https://localhost:6001/test");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
}
else
{
    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine(JArray.Parse(content));
}
Console.ReadKey();
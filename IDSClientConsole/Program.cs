using IdentityModel.Client;
using Newtonsoft.Json.Linq;

await GetTokenAndCallApiAsync();

static async Task GetTokenAndCallApiAsync()
{
    // discover endpoints from metadata
    var client = new HttpClient();
    var disco = await client.GetDiscoveryDocumentAsync("http://localhost:4010");
    if (disco.IsError)
    {
        Console.WriteLine(disco.Error);
        return;
    }

    var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = disco.TokenEndpoint,

        ClientId = "consoleclient",
        ClientSecret = "secret1",
        Scope = "myapi"
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

    var response = await apiClient.GetAsync("http://localhost:5153/WeatherForecast");
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine(response.StatusCode);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
    else
    {
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JArray.Parse(content));
    }

    Console.ReadLine();
}
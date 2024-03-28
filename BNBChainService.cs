using System.Numerics;
using System.Text.Json;

public class BNBChainService
{
    private readonly string _contractAddress = "0xfE1d7f7a8f0bdA6E415593a2e4F82c64b446d404";
    private readonly string _apiKey;
    private readonly string _apiURL = "https://api.bscscan.com/api";

    public BNBChainService(IConfiguration configuration)
    {
        _apiKey = configuration["bscscan:ApiKey"] ?? "";
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("API key is missing or empty. Please provide a valid API key.");
        }
    }

    public async Task<BigInteger> GetTotalSupplyAsync()
    {
        string url = $"{_apiURL}?module=stats&action=tokensupply&contractaddress={_contractAddress}&apikey={_apiKey}";

        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<ApiResult>(responseBody);

                if (responseObject?.status == "1" && BigInteger.TryParse(responseObject.result, out BigInteger totalSupply))
                {
                    return totalSupply;
                }
                else
                {
                    Console.WriteLine($"API error: {responseObject?.message ?? "Unknown"}");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Failed to fetch data: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON response: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return BigInteger.Zero;
    }

    public async Task<BigInteger> GetNonCirculatingSupplyAsync()
    {
        var nonCirculatingAddresses = new List<string>
        {
            "0x000000000000000000000000000000000000dEaD",
            "0xe9e7CEA3DedcA5984780Bafc599bD69ADd087D56",
            "0xfE1d7f7a8f0bdA6E415593a2e4F82c64b446d404",
            "0x71F36803139caC2796Db65F373Fb7f3ee0bf3bF9",
            "0x62D6d26F86F2C1fBb65c0566Dd6545ae3F9A63F1",
            "0x83a7152317DCfd08Be0F673Ab614261b4D1e1622",
            "0x5A749B82a55f7d2aCEc1d71011442E221f55A537",
            "0x9eBbBE47def2F776D6d2244AcB093AB2fD1B2C2A",
            "0xcdD80c6F317898a8aAf0ec7A664655E25E4833a2",
            "0x456F20bb4d89d10A924CE81b7f0C89D5711CE05B"
        };

        var totalNonCirculatingSupply = BigInteger.Zero;

        try
        {
            var tasks = nonCirculatingAddresses.Select(address => GetTokenBalanceAsync(address));
            var balances = await Task.WhenAll(tasks);
            totalNonCirculatingSupply = balances.Aggregate(BigInteger.Add);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return totalNonCirculatingSupply;
    }

    private async Task<BigInteger> GetTokenBalanceAsync(string address)
    {
        try
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{_apiURL}?module=account&action=tokenbalance&contractaddress={_contractAddress}&address={address}&tag=latest&apikey={_apiKey}");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<ApiResult>(responseBody);
                if (responseObject?.status == "1")
                {
                    return BigInteger.Parse(responseObject.result ?? "");
                }
                else
                {
                    Console.WriteLine($"API error: {responseObject?.message ?? "Unknown"}");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Failed to fetch data: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON response: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return BigInteger.Zero;
    }

    private class ApiResult
    {
        public string? status { get; set; }
        public string? message { get; set; }
        public string? result { get; set; }
    }

}
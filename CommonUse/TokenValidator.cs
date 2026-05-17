using System.Net.Http.Headers;
using System.Net.Http.Json;
using AuthenticationService.EntityModel;
using Microsoft.Extensions.Logging;



namespace CommonUse
{
    public class TokenValidator
    {
        private readonly ILogger<TokenValidator> _logger;
        public TokenValidator(ILogger<TokenValidator> logger)
        {
            _logger = logger;
        }
        public async Task<AuthResponse> Validate(string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:7010");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync("/api/accounts/validate");
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Validation Successful");
                    var user = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    return user;
                }
                else
                {
                    _logger.LogError("Validation Failed");
                }
            }
            return null;
        }
    }
}

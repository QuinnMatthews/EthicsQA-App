using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json;

namespace EthicsQA.Services {
    public class EthicsAPIClient {
        private readonly HttpClient httpClient;
        private readonly ExternalAuthStateProvider externalAuthStateProvider;

        public EthicsAPIClient(AuthenticationStateProvider authProvider) {
            string baseURL = "https://ethicsqa-api.azurewebsites.net";
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseURL);
           
            externalAuthStateProvider = (ExternalAuthStateProvider) authProvider;
        }

        public async Task<SendAuthenticationCodeResult> SendAuthenticationCodeAsync(string phone) {
            // Call the API to send the code
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/GetSMSCode");
            
            // Set the body of the request
            request.Content = new StringContent($"{{\"phone\":\"{phone}\"}}", Encoding.UTF8, "application/json");
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await httpClient.SendAsync(request);

            if(response.IsSuccessStatusCode) {
                return new SendAuthenticationCodeResult() { Success = true };
            } else {
                var _message = await response.Content.ReadAsStringAsync();
                return new SendAuthenticationCodeResult() { Success = false, ErrorMessage = _message };
            }
        }

        public async Task<LoginResult> LoginAsync(string phone, string code) {
            // Call the API to exchange the code for a token
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/ExchangeCode");
            request.Content = new StringContent($"{{\"phone\":\"{phone}\",\"code\":\"{code}\"}}", Encoding.UTF8, "application/json");
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await httpClient.SendAsync(request);

            if(!response.IsSuccessStatusCode) {
                var _message = await response.Content.ReadAsStringAsync();
                return new LoginResult() { Success = false, ErrorMessage = _message };
            }

            var body = await response.Content.ReadAsStringAsync();
            // Get access_token from the response
            var body_dict = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
            var jwt = body_dict["access_token"];
            var payload = jwt.Split('.')[1];
            var jsonBytes = Convert.FromBase64String(payload);
            var json = Encoding.UTF8.GetString(jsonBytes);
            var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                .Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));

            var identity = new ClaimsIdentity(claims, "serverauth");
            var user = new ClaimsPrincipal(identity);
            externalAuthStateProvider.SetUser(user);
            return new LoginResult() { Success = true, User = user };
        }

    }

    public class LoginResult {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public ClaimsPrincipal User { get; set; }
    }

    public class SendAuthenticationCodeResult {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        
    }
}

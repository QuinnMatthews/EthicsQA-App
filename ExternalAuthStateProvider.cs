using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EthicsQA {

    using System.Security.Claims;
    using System.Threading.Tasks;
    using EthicsQA.Services;
    using Microsoft.AspNetCore.Components.Authorization;

    public class ExternalAuthStateProvider : AuthenticationStateProvider {
        private ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly EthicsAPIClient apiClient;

        public ExternalAuthStateProvider(EthicsAPIClient client) {
            apiClient = client;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(new AuthenticationState(currentUser));

        public Task LogInAsync() {
            var loginTask = LogInAsyncCore();
            NotifyAuthenticationStateChanged(loginTask);

            return loginTask;

            async Task<AuthenticationState> LogInAsyncCore() {
                var user = await LoginWithAuth0Async();
                currentUser = user;

                return new AuthenticationState(currentUser);
            }
        }

        private async Task<ClaimsPrincipal> LoginWithAuth0Async() {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
            var loginResult = await apiClient.LoginAsync();

            if(!loginResult.IsError) {
                authenticatedUser = loginResult.User;
            }
            return authenticatedUser;
        }

        public async void LogOut() {
            await apiClient.LogoutAsync();
            currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(currentUser)));
        }
    }
}

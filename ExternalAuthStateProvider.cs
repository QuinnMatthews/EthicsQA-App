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

        public ExternalAuthStateProvider() { 

        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(new AuthenticationState(currentUser));


        public void SetUser(ClaimsPrincipal user) {
            currentUser = user;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(currentUser)));
        }

        public void LogOut() {
            currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(currentUser)));
        }
    }
}

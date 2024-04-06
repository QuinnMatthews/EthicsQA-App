using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace EthicsQA.Services {
    public class EthicsAPIClient {
        public EthicsAPIClient() {
        }

        public async Task<LoginResult> LoginAsync() {
            return new LoginResult();
        }

        public async Task LogoutAsync() {
        }


    }

    public class LoginResult {
        public bool IsError { get; set; }
        public ClaimsPrincipal User { get; set; }
    }
}

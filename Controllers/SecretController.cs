using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VaultSharp;

namespace HashicorpVaultAppRoleAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretController : ControllerBase
    {
        private readonly IVaultClient _vaultClient;
        public SecretController(IVaultClient vaultClient) {
            this._vaultClient = vaultClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {
            var result = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("hello", mountPoint: "secret").ConfigureAwait(false);
            return Ok(result);
        }
    }
}

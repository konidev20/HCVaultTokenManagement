using HashicorpVaultAppRoleAuthentication.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.Core;

namespace HashicorpVaultAppRoleAuthentication.BackgroundServices
{
    public class HashiCorpVaultLoginService : BackgroundService
    {
        private readonly ILogger<HashiCorpVaultLoginService> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        private IVaultClient _vaultClient { get; set; }

        private int TokenTTL { get; set; }
        private bool IsLoggedIn { get; set; } = false;

        private int _loginCount { get; set; } = 0;

        public HashiCorpVaultLoginService(ILogger<HashiCorpVaultLoginService> logger,
                                          IHostApplicationLifetime hostApplicationLifetime,
                                          IVaultClient vaultClient,
                                          HCVaultTokenConfiguration hcVaultTokenConfiguration) {
            this._logger = logger;
            this._hostApplicationLifetime = hostApplicationLifetime;
            this._vaultClient = vaultClient;

            TokenTTL = hcVaultTokenConfiguration.TokenTTL; 
        }

        public override async Task StartAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Start : HashiCorp Vault Login Service");
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                try {
                    if (IsLoggedIn && _loginCount > 0) {
                        _vaultClient.V1.Auth.ResetVaultToken();
                        await _vaultClient.V1.Auth.PerformImmediateLogin();
                        _logger.LogInformation($"Requested new token from HashiCorp Vault");
                        _loginCount++;
                    }
                    if (!IsLoggedIn && _loginCount == 0) {
                        await _vaultClient.V1.Auth.PerformImmediateLogin();
                        _logger.LogInformation($"Intial Login Complete");
                        _loginCount++;
                        IsLoggedIn = true;
                    }
                } catch (VaultApiException ve) {
                    _logger.LogInformation($"HC Vault Login Failed : {ve.Message}");
                    _logger.LogInformation("Terminating the application");
                    _hostApplicationLifetime.StopApplication();
                } 
                await Task.Delay(TimeSpan.FromMinutes(TokenTTL));
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("Stop : HashiCorp Vault Login Service");
            return base.StopAsync(cancellationToken);
        }
    }
}

using HashicorpVaultAppRoleAuthentication.BackgroundServices;
using HashicorpVaultAppRoleAuthentication.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;

namespace HashicorpVaultAppRoleAuthentication
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HashicorpVaultAppRoleAuthentication", Version = "v1" });
            });
            services.AddSingleton(Configuration.GetSection("HCVaultConfiguration").Get<HCVaultConfiguration>());
            services.AddSingleton(Configuration.GetSection("HCVaultTokenConfiguration").Get<HCVaultTokenConfiguration>());
            services.AddSingleton<IVaultClient,VaultClient>(sp => {
                var hcVaultConfiguration = sp.GetRequiredService<HCVaultConfiguration>();
                IAuthMethodInfo authMethod = new AppRoleAuthMethodInfo(hcVaultConfiguration.RoleId, hcVaultConfiguration.SecretId);
                VaultClientSettings vaultClientSettings = new VaultClientSettings(hcVaultConfiguration.VaultAddress, authMethod);
                return new VaultClient(vaultClientSettings);
            });
            services.AddSingleton<IHostedService, HashiCorpVaultLoginService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HashicorpVaultAppRoleAuthentication v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}

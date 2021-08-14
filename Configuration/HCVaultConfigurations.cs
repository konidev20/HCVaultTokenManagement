namespace HashicorpVaultAppRoleAuthentication.Configuration
{
    public class HCVaultConfiguration
    {
        public string VaultAddress { get; set; }
        public string RoleId { get; set; }
        public string SecretId { get; set; }
    }

    public class HCVaultTokenConfiguration
    {
        public int TokenTTL { get; set; }
    }
}

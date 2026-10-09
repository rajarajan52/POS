using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods for Azure Key Vault configuration integration.
/// </summary>
internal static class AzureKeyVaultConfigurationExtensions
{
    /// <summary>
    /// Adds Azure Key Vault as a configuration source.
    /// </summary>
    public static IConfigurationBuilder AddAzureKeyVault(
        this IConfigurationBuilder builder,
        Uri vaultUri,
        TokenCredential credential)
    {
        var secretClient = new SecretClient(vaultUri, credential);
        builder.Add(new AzureKeyVaultConfigurationSource(secretClient));
        return builder;
    }
}

/// <summary>
/// Configuration source for Azure Key Vault.
/// </summary>
internal class AzureKeyVaultConfigurationSource : IConfigurationSource
{
    private readonly SecretClient _secretClient;

    public AzureKeyVaultConfigurationSource(SecretClient secretClient)
    {
        _secretClient = secretClient;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AzureKeyVaultConfigurationProvider(_secretClient);
    }
}

/// <summary>
/// Configuration provider that reads secrets from Azure Key Vault.
/// </summary>
internal class AzureKeyVaultConfigurationProvider : ConfigurationProvider
{
    private readonly SecretClient _secretClient;

    public AzureKeyVaultConfigurationProvider(SecretClient secretClient)
    {
        _secretClient = secretClient;
    }

    public override void Load()
    {
        try
        {
            var secretsPageable = _secretClient.GetPropertiesOfSecretsAsync();
            var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            var loadTask = Task.Run(async () =>
            {
                await foreach (var secretProperty in secretsPageable.AsPages())
                {
                    foreach (var secret in secretProperty.Values)
                    {
                        var secretValue = await _secretClient.GetSecretAsync(secret.Name);
                        data[secret.Name] = secretValue.Value.Value;
                    }
                }
            });

            loadTask.Wait();
            Data = data;
        }
        catch (Exception ex)
        {
            // Log but don't throw - allows graceful fallback in non-production scenarios
            System.Diagnostics.Debug.WriteLine($"Error loading Key Vault secrets: {ex.Message}");
        }
    }
}


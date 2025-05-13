// DataProtectionConfig.cs
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.DataProtection;

namespace VoxDocs.Configurations
{
    public static class DataProtectionConfig
    {
        public static IServiceCollection AddCustomDataProtection(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var blobConn  = configuration["DataProtection:BlobConnectionString"];
            var container = configuration["DataProtection:BlobContainerName"];
            var blobName  = configuration["DataProtection:BlobName"];

            var dpBuilder = services
                .AddDataProtection()
                .SetApplicationName("VoxDocs");

            if (!string.IsNullOrEmpty(blobConn) &&
                !string.IsNullOrEmpty(container) &&
                !string.IsNullOrEmpty(blobName))
            {
                try
                {
                    var containerClient = new BlobContainerClient(blobConn, container);
                    containerClient.CreateIfNotExists(PublicAccessType.None);

                    // CORREÇÃO: use o BlobClient sobrecarregado
                    var blobClient = containerClient.GetBlobClient(blobName);
                    dpBuilder.PersistKeysToAzureBlobStorage(blobClient);

                    Console.WriteLine("[DataProtection] Persistindo chaves no Azure Blob Storage.");
                    return services;
                }
                catch (RequestFailedException ex) when (
                           ex.ErrorCode == BlobErrorCode.AccountIsDisabled ||
                           ex.ErrorCode == BlobErrorCode.AuthenticationFailed ||
                           ex.ErrorCode == BlobErrorCode.AuthorizationPermissionMismatch)
                {
                    Console.WriteLine($"[DataProtection] BlobStorage indisponível ({ex.ErrorCode}), fallback local.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DataProtection] Erro no BlobStorage: {ex.Message}, fallback local.");
                }
            }

            // Fallback para disco local
            var keysFolder = Path.Combine(AppContext.BaseDirectory, "DataProtection-Keys");
            Directory.CreateDirectory(keysFolder);
            dpBuilder.PersistKeysToFileSystem(new DirectoryInfo(keysFolder));
            Console.WriteLine($"[DataProtection] Persistindo chaves em disco: {keysFolder}");

            return services;
        }
    }
}

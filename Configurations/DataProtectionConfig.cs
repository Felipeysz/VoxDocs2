using Azure.Storage.Blobs;
using Microsoft.AspNetCore.DataProtection;

namespace VoxDocs.Configurations
{
    public static class DataProtectionConfig
    {
        public static IServiceCollection AddCustomDataProtection(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var dp = configuration.GetSection("DataProtection");
            var blobConn  = dp["BlobConnectionString"];
            var container = dp["BlobContainerName"];
            var blobName  = dp["BlobName"];

            var client = new BlobContainerClient(blobConn, container);
            client.CreateIfNotExists();

            services.AddDataProtection()
                .SetApplicationName("VoxDocs")
                .PersistKeysToAzureBlobStorage(
                    client.GetBlobClient(blobName));

            return services;
        }
    }
}

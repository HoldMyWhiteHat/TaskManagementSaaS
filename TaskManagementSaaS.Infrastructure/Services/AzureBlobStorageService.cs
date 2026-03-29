using TaskManagementSaaS.Application.Interfaces;

namespace TaskManagementSaaS.Infrastructure.Services
{
    public class AzureBlobStorageService : IBlobStorageService
    {
        public async Task<string> UploadAsync(Stream content, string fileName, string contentType)
        {
            // Placeholder: Not fully implemented in this phase
            return await Task.FromResult("");
        }

        public async Task DeleteAsync(string blobUri)
        {
            await Task.CompletedTask;
        }
    }
}

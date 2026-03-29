namespace TaskManagementSaaS.Application.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(Stream content, string fileName, string contentType);
        Task DeleteAsync(string blobUri);
    }
}

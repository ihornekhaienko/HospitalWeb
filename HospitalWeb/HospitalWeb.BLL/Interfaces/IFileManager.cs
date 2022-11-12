using Microsoft.AspNetCore.Http;

namespace HospitalWeb.Services.Interfaces
{
    public interface IFileManager : IDisposable
    {
        public Task<string> UploadToServer(byte[] file, string path);
        public Task<string> UploadToServer(IFormFile file, string path);
        public Task<byte[]> GetBytes(IFormFile file);
        public Task<byte[]> GetBytes(string path);
        public void DeleteFile(string path);
    }
}

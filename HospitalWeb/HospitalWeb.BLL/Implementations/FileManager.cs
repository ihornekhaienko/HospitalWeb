using HospitalWeb.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HospitalWeb.Services.Implementations
{
    internal class FileManager : IFileManager
    {
        private string _filePath;

        public async Task<byte[]> GetBytes(IFormFile file)
        {
            Stream stream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task<byte[]> GetBytes(string path)
        {
            using FileStream fstream = File.OpenRead(path);
            byte[] buffer = new byte[fstream.Length];
            await fstream.ReadAsync(buffer, 0, buffer.Length);

            return buffer;
        }

        public async Task<string> UploadToServer(byte[] file, string path)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = $@"{DateTime.Now.Ticks}";
                _filePath = Path.Combine(path, fileName);
                await File.WriteAllBytesAsync(_filePath, file);

                return _filePath;
            }

            return null;
        }

        public async Task<string> UploadToServer(IFormFile file, string path)
        {
            if (file != null && file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName).Substring(1);
                var fileName = $@"{DateTime.Now.Ticks}.{extension}";
                _filePath = Path.Combine(path, fileName);

                using (var fileStream = new FileStream(_filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return _filePath;
            }

            return null;
        }

        public void DeleteFile(string path)
        {
            if (path != null && File.Exists(path))
            {
                File.Delete(path);
            }
        }

        #region Dispose
        bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                }
            }
            //dispose unmanaged resources
            DeleteFile(_filePath);
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

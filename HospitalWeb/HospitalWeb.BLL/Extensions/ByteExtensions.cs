namespace HospitalWeb.Services.Extensions
{
    public static class ByteExtensions
    {
        public static bool IsImage(this byte[] fileBytes)
        {
            if (fileBytes.Length < 2)
            {
                return false;
            }

            var headers = new List<byte[]>
        {
            new byte[] { 0x42, 0x4D }, // BMP
            new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, // GIF
            new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, // GIF
            new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, // PNG
            new byte[] { 0x49, 0x49, 0x2A, 0x00 }, // TIFF
            new byte[] { 0x4D, 0x4D, 0x00, 0x2A }, // TIFF
            new byte[] { 0xFF, 0xD8, 0xFF }, // JPEG
            new byte[] { 0xFF, 0xD9 }, // JPEG
        };

            return headers.Any(x => x.SequenceEqual(fileBytes.Take(x.Length)));
        }
    }
}

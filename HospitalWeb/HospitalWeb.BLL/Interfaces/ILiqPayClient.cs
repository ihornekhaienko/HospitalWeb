using HospitalWeb.Services.Utility;

namespace HospitalWeb.Services.Interfaces
{
    public interface ILiqPayClient
    {
        public Task<LiqPayResponse> RequestAsync(string path, LiqPayRequest request);
    }
}

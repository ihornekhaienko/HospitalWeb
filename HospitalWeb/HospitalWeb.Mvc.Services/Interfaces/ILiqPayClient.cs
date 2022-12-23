using HospitalWeb.Mvc.Services.Utility;

namespace HospitalWeb.Mvc.Services.Interfaces
{
    public interface ILiqPayClient
    {
        public Task<LiqPayResponse> RequestAsync(string path, LiqPayRequest request);
    }
}

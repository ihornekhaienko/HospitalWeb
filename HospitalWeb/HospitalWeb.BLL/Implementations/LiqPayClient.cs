using HospitalWeb.Services.Interfaces;
using HospitalWeb.Services.Utility;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace HospitalWeb.Services.Implementations
{
    public class LiqPayClient : ILiqPayClient
    {
        private readonly string _privateKey;
        private readonly string _publicKey;
        private readonly string _apiUrl;

        public LiqPayClient(IConfiguration config)
        {
            _privateKey = config["LiqPay:PrivateKey"];
            _publicKey = config["LiqPay:PublicKey"];
            _apiUrl = config["LiqPay:ApiUrl"];
        }

        public async Task<LiqPayResponse> RequestAsync(string path, LiqPayRequest request)
        {
            var data = CreateRequestData(request);

            string response = await PostAsync(_apiUrl + path, data);

            return JsonConvert.DeserializeObject<LiqPayResponse>(response);
        }

        private Dictionary<string, string> CreateRequestData(LiqPayRequest request)
        {
            request.Version = 3;
            request.PublicKey = _publicKey;

            string jsonString = JObject.FromObject(request).ToString();
            string data = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

            var requestData = new Dictionary<string, string>();
            requestData.Add("data", data);
            requestData.Add("signature", CreateSignature(data));

            return requestData;
        }

        private string CreateSignature(string base64Encoded)
        {
            base64Encoded = _privateKey + base64Encoded + _privateKey;
            using var sha1 = SHA1.Create();

            return Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(base64Encoded)));
        }

        private async Task<string> PostAsync(string url, Dictionary<string, string> data)
        {
            string urlParameters;
            var parameters = new List<string>();
            foreach (var item in data)
            {
                var queryValue = WebUtility.HtmlEncode(item.Value);
                byte[] bytes = Encoding.Default.GetBytes(queryValue);
                var utf8QueryValue = Encoding.UTF8.GetString(bytes);

                parameters.Add($"{item.Key}={utf8QueryValue}");
            }
            urlParameters = string.Join("&", parameters);

            using var httpClient = new HttpClient();
            using var response = await httpClient.PostAsync(url, new StringContent(urlParameters));

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(responseStream, Encoding.GetEncoding(Encoding.UTF8.CodePage));

            return reader.ReadToEnd();
        }
    }
}

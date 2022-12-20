using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HospitalWeb.Services.Utility
{
    public class LiqPayRequest
    {
        [JsonProperty("version")]
        public int Version { get; set; }
        [JsonProperty("public_key")]
        public string PublicKey { get; set; }
        [JsonProperty("action")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LiqPayRequestAction? Action { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        [JsonProperty("card")]
        public string Card { get; set; }
        [JsonProperty("card_cvv")]
        public string CardCvv { get; set; }
        [JsonProperty("card_exp_month")]
        public string CardExpiredMonth { get; set; }
        [JsonProperty("card_exp_year")]
        public string CardExpiredYear { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("order_id")]
        public string OrderId { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}

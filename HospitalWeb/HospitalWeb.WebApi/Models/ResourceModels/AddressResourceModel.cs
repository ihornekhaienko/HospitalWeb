namespace HospitalWeb.WebApi.Models.ResourceModels
{
    public class AddressResourceModel
    {
        public int? AddressId { get; set; }

        public string FullAddress { get; set; }

        public int LocalityId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace HospitalWeb.ViewModels.Treatment
{
    public class PaymentViewModel
    {
        [Required(ErrorMessage = "AmountRequired")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "CardRequired")]
        public string Card { get; set; }

        [Required(ErrorMessage = "ExpirationRequired")]
        public string CardExpirationMonth { get; set; }

        [Required(ErrorMessage = "ExpirationRequired")]
        public string CardExpirationYear { get; set; }

        [Required(ErrorMessage = "CvvRequired")]
        public string Cvv { get; set; }

        [Required(ErrorMessage = "PaymentDescRequired")]
        public string Description { get; set; }

        [Required(ErrorMessage = "PhoneRequired")]
        [RegularExpression("^\\+?[1-9][0-9]{7,14}$")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }
    }
}

using HospitalWeb.Mvc.Attributes.Validation;

namespace System.ComponentModel.DataAnnotations
{
    public class DateOfBirthAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime)value;

            var comparisonValue = DateTime.Today.AddYears(-18);

            if (currentValue > comparisonValue)
                return new ValidationResult(GetErrorMessage(validationContext));

            return ValidationResult.Success;
        }

        private string GetErrorMessage(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                return "Invalid CPF";
            }

            var errorTranslation = validationContext.GetService(typeof(ErrorMessageTranslationService)) as ErrorMessageTranslationService;

            return errorTranslation.GetLocalizedError(ErrorMessage);
        }
    }
}

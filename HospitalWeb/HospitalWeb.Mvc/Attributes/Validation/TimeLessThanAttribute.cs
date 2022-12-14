using HospitalWeb.Mvc.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace System.ComponentModel.DataAnnotations
{
    public class TimeLessThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public TimeLessThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

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

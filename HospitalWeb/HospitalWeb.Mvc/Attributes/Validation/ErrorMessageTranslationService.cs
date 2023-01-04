using Microsoft.Extensions.Localization;

namespace HospitalWeb.Mvc.Attributes.Validation
{
    public class ErrorMessageTranslationService
    {
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        public ErrorMessageTranslationService(IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _sharedLocalizer = sharedLocalizer;
        }

        public string GetLocalizedError(string errorKey)
        {
            return _sharedLocalizer[errorKey];
        }
    }
}

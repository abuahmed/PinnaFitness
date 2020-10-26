using System.ComponentModel.DataAnnotations;

namespace PinnaFit.Core.CustomValidationAttributes
{
    public class GreaterThanZero : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //int ourResult = 0;
            //if (value != null)
            //{
            //    bool converted = int.TryParse(value.ToString(), out ourResult);
            //    if (converted && ourResult > 0)
            //    {
            //        return ValidationResult.Success;
            //    }
            //    else
            //    {
            //        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            //    }
            //}
            return ValidationResult.Success;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace MachinesDataAnalyzingSystem.Core.Helpers;

public static class ValidationHelper
{
    public static void ModelValidation(object obj)
    {
        List<ValidationResult> validationResults = [];
        if(!Validator.TryValidateObject(obj, new(obj), validationResults, true))
        {
            throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
        }
    }
}

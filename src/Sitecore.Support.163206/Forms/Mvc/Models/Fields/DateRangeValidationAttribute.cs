using Sitecore.Forms.Mvc.Validators;
using System;
using System.ComponentModel.DataAnnotations;
using Sitecore.Forms.Mvc.Interfaces;
using Sitecore.Forms.Mvc.Extensions;

namespace Sitecore.Support.Forms.Mvc.ViewModels.Fields
{
  public class DateRangeValidationAttribute : DynamicValidationBase
  {
    protected override ValidationResult ValidateFieldValue(IViewModel model, object value, ValidationContext validationContext)
    {
      int day = TransformStringToInt(model.GetPropertyValue<object>("Day"));
      int month = TransformStringToInt(model.GetPropertyValue<object>("Month"));
      int year = TransformStringToInt(model.GetPropertyValue<object>("Year"));
      DateTime startDate = model.GetPropertyValue<DateTime>("StartDate");
      DateTime endDate = model.GetPropertyValue<DateTime>("EndDate");
      
      string errorMessage = string.Format("{0} must be later than {1} and before {2}.", model.Title, startDate.ToString("D"), endDate.Date.ToString("D"));
      if (day != 0 && month != 0 && year != 0)
      {
        DateTime _enteredDate = new DateTime(year, month, day);
        if (_enteredDate >= startDate && _enteredDate <= endDate)
          return ValidationResult.Success;
        else
          return new ValidationResult(errorMessage);
      }
      else
        return new ValidationResult(errorMessage);
    }

    private int TransformStringToInt(object obj)
    {
      int num = 0;
      if (obj is string[])
      {
        int.TryParse(((string[])obj)[0], out num);
      }
      else
      {
        int.TryParse(obj.ToString(), out num);
      }
      return num;
    }
  }
}
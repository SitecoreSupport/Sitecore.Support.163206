using Sitecore.Forms.Mvc.Extensions;
using Sitecore.Forms.Mvc.Models;
using Sitecore.Forms.Mvc.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace Sitecore.Support.Forms.Mvc.Models.Fields
{
    public class DateRangeValidationAttribute : DynamicValidationBase
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            FieldModel model = base.GetModel(validationContext);
            var day = (string[])model.GetPropertyValue<object>("Day");
            var month = (string[])model.GetPropertyValue<object>("Month");
            var year = (string[])model.GetPropertyValue<object>("Year");
            DateTime startDate = model.GetPropertyValue<DateTime>("StartDate");
            DateTime endDate = model.GetPropertyValue<DateTime>("EndDate");
            string errorMessage = String.Format("Field must be later than {0} and before {1}", startDate.ToString("D"), endDate.Date.ToString("D"));
            if (day != null && month != null && year != null)
            {
                DateTime _enteredDate = new DateTime(Int32.Parse(year[0]), Int32.Parse(month[0]), Int32.Parse(day[0]));
                if (_enteredDate >= startDate && _enteredDate <= endDate)
                    return ValidationResult.Success;
                else
                    return new ValidationResult(errorMessage);
            }
            else
                return new ValidationResult(errorMessage);
        }
    }
}
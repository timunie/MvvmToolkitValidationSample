using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmToolkitValidationSample
{
    public class ViewModel : ObservableValidator
    {
        public ViewModel()
        {
            ValidateAllProperties();
        }

        private string _StringMayNotBeEmpty;
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field {0} may not be empty.")]
        public string StringMayNotBeEmpty
        {
            get { return _StringMayNotBeEmpty; }
            set { SetProperty(ref _StringMayNotBeEmpty, value, true); }
        }



        private string _AnotherRequiredField;
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field {0} may not be empty.")]
        public string AnotherRequiredField
        {
            get { return _AnotherRequiredField; }
            set { SetProperty(ref _AnotherRequiredField, value, true); }
        }



        private int _ValueMayNotExceedBounds;
        [DynamicRange(nameof(LowerBound), nameof(UpperBound), ErrorMessage = "The property is not in a valid Range")]
        public int ValueMayNotExceedBounds
        {
            get { return _ValueMayNotExceedBounds; }
            set { SetProperty(ref _ValueMayNotExceedBounds, value, true); }
        }

        private int _LowerBound = 0;
        public int LowerBound
        {
            get { return _LowerBound; }
            set { SetProperty(ref _LowerBound, value, true); ValidateProperty(ValueMayNotExceedBounds, nameof(ValueMayNotExceedBounds)); }
        }

        private int _UpperBound = 10;
        public int UpperBound
        {
            get { return _UpperBound; }
            set { SetProperty(ref _UpperBound, value, true); ValidateProperty(ValueMayNotExceedBounds, nameof(ValueMayNotExceedBounds)); }
        }


        private int _ValidateAsyncExample;
        [CustomValidation(typeof(ViewModel), nameof(LongRunningValidation))]
        public int ValidateAsyncExample
        {
            get { return _ValidateAsyncExample; }
            set { SetProperty(ref _ValidateAsyncExample, value, true); }
        }


        public static ValidationResult LongRunningValidation(int value, ValidationContext _)
        {
            if (value % 2 == 0)
            {
                return ValidationResult.Success;
            }
            else
            {
                Task.Delay(1000).Wait();
                return new ValidationResult("The Value must be even.");
            }
        }
    }

    public class DynamicRangeAttribute : ValidationAttribute
    {
        private readonly string _lowerBoundProperty;
        private readonly string _upperBoundProperty;
        public DynamicRangeAttribute(string lowerBoundPropertyName, string upperBoundPropertyName)
        {
            _lowerBoundProperty = lowerBoundPropertyName;
            _upperBoundProperty = upperBoundPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var lowerBoundProperty = validationContext.ObjectType.GetProperty(_lowerBoundProperty);
            var upperBoundProperty = validationContext.ObjectType.GetProperty(_upperBoundProperty);

            if (lowerBoundProperty is null)
            {
                return new ValidationResult(
                    string.Format("Unknown property: {0}", _lowerBoundProperty)
                );
            }

            if (upperBoundProperty is null)
            {
                return new ValidationResult(
                    string.Format("Unknown property: {0}", _upperBoundProperty)
                );
            }

            var lowerValue = (int)lowerBoundProperty.GetValue(validationContext.ObjectInstance, null);
            var upperValue = (int)upperBoundProperty.GetValue(validationContext.ObjectInstance, null);


            if (value is int intVal && lowerValue <= intVal && upperValue >= intVal)
            {
                // If the value is in Range we return null
                return null;
            }
            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
        }

        public override bool RequiresValidationContext => true;
    }
}

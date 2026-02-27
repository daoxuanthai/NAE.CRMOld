using System;
using System.Web.Mvc;

namespace tcs.adapter.Helper
{
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            object result = null;
            
            var modelName = bindingContext.ModelName;
            var attemptedValue = bindingContext.ValueProvider.GetValue(modelName).AttemptedValue;

            try
            {
                if (bindingContext.ModelMetadata.IsNullableValueType
                    && string.IsNullOrWhiteSpace(attemptedValue))
                {
                    return null;
                }
                attemptedValue = attemptedValue.Replace(",", "").Replace(".", "");
                result = !string.IsNullOrEmpty(attemptedValue) ? Convert.ToDecimal(attemptedValue) : 0;
            }
            catch (FormatException e)
            {
                bindingContext.ModelState.AddModelError(modelName, e);
            }

            return result;
        }
    }
}

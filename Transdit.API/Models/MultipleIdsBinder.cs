using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace Transdit.API.Models
{
    public class MultipleIdsBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var data = bindingContext.HttpContext.Request.Query;
            var result = data.TryGetValue("ids", out StringValues ids);

            if (result)
            {
                var idList = ids.ToString().Split("|");
                bindingContext.Result = ModelBindingResult.Success(idList);
            }
            else bindingContext.Result = ModelBindingResult.Failed();

            return Task.CompletedTask;
        }
    }
}

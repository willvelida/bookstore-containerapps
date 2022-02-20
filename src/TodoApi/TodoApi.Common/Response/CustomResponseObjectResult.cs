using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Common.Response
{
    public class CustomRequestObjectResult : ActionResult, IActionResult
    {
        private readonly object _result;
        private readonly int _returnStatusCode;

        public CustomRequestObjectResult(object result, int returnStatusCode)
        {
            _result = result;
            _returnStatusCode = returnStatusCode;
        }
    }
}

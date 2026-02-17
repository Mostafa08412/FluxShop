using FluxStore.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FluxStore.Api.Infrastructure
{


    public class BaseController : ControllerBase
    {

        internal ISender _sender;

        public BaseController(ISender sender)
        {
            _sender = sender;
        }

        protected IActionResult HandleResult<T>(Result<T> result, ApplicationStatusCodes onSuccess)
        {
            var helper = new ApiResponseHelper();
            var response = helper.ResultToResponse(result, HttpContext);
            var statusCode = helper.CalculateStatusCodeFromResult(result, onSuccess);
            return StatusCode((int)statusCode, response);

        }
        protected IActionResult HandleResult(Result result, ApplicationStatusCodes onSuccess)
        {
            var helper = new ApiResponseHelper();
            var response = helper.ResultToResponse(result, HttpContext);
            var statusCode = helper.CalculateStatusCodeFromResult(result, onSuccess);
            return StatusCode((int)statusCode, response);

        }

    }


}


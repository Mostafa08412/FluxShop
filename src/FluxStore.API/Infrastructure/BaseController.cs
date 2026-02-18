using FluxStore.Application.Common.Resources;
using FluxStore.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FluxStore.Api.Infrastructure
{


    public class BaseController : ControllerBase
    {

        internal ISender _sender;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public BaseController(ISender sender, IStringLocalizer<SharedResource> localizer)
        {
            _sender = sender;
            _localizer = localizer;
        }

        protected IActionResult HandleResult<T>(Result<T> result, ApplicationStatusCodes onSuccess)
        {
            var helper = new ApiResponseHelper(_localizer);
            var response = helper.ResultToResponse(result, HttpContext);
            var statusCode = helper.CalculateStatusCodeFromResult(result, onSuccess);
            return StatusCode((int)statusCode, response);

        }
        protected IActionResult HandleResult(Result result, ApplicationStatusCodes onSuccess)
        {
            var helper = new ApiResponseHelper(_localizer);
            var response = helper.ResultToResponse(result, HttpContext);
            var statusCode = helper.CalculateStatusCodeFromResult(result, onSuccess);
            return StatusCode((int)statusCode, response);

        }

    }


}


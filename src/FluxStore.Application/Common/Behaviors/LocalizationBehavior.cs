using FluxStore.Application.Common.Resources;
using FluxStore.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Localization;

namespace FluxStore.Application.Common.Behaviors
{
    public class LocalizationBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes> where TReq : notnull
    {

        private readonly IStringLocalizer<SharedResource> localizer;

        public LocalizationBehavior(IStringLocalizer<SharedResource> localizer)
        {
            this.localizer = localizer;
        }
        public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken cancellationToken)
        {
            var response = await next();

            if (response is IResult result)
            {
                if (result.Errors.Any())
                {

                    foreach (var error in result.Errors)
                    {
                        var localized = localizer.GetString(error.Code);
                        if (!localized.ResourceNotFound)
                        {
                            error.Description = localized.Value;
                        }
                    }

                }

            }

            return response;



        }
    }
}

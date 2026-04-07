using Asp.Versioning.Builder;
using FastEndpoints;

namespace FluxStore.Api.Extensions
{
    public static class ApiGroups
    {


        public class AuthenticationV1Group : Group
        {
            public const string GroupName = nameof(ApiRoutes.Authentication);
            public const string GroupPrefix = ApiRoutes.Authentication;

            public AuthenticationV1Group()
            {
                Configure(GroupPrefix, ep =>
                {
                    ep.Description(x => x
                        .WithApiVersionSet(new ApiVersionSetBuilder(GroupName).Build())
                        .MapToApiVersion(1.0));
                });
            }
        }

        public class AccountV1Group : Group
        {
            public const string GroupName = nameof(ApiRoutes.Profile);
            public const string GroupPrefix = ApiRoutes.Versioned;

            public AccountV1Group()
            {
                Configure(GroupPrefix, ep =>
                {
                    ep.Description(x => x
                        .WithApiVersionSet(new ApiVersionSetBuilder(GroupName).Build())
                        .MapToApiVersion(1.0));
                });
            }
        }
    }
}

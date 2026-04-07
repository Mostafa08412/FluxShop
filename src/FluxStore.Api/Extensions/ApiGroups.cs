using FastEndpoints;

namespace FluxStore.Api.Extensions
{
    public static class ApiGroups
    {


        public class AuthenticationGroup : Group
        {
            private const string GroupName = nameof(ApiRoutes.Authentication);
            private const string GroupPrefix = ApiRoutes.Authentication.Prefix;

            public AuthenticationGroup()
            {
                Configure(GroupPrefix, ep =>
                {
                    ep.Tags(GroupName);
                    ep.Description(x => x.WithTags(GroupName)
              );
                });
            }
        }

        public class AccountGroup : Group
        {
            private const string GroupName = nameof(ApiRoutes.Account);
            private const string GroupPrefix = ApiRoutes.Account.Prefix;

            public AccountGroup()
            {
                Configure(GroupPrefix, ep =>
                {
                    ep.Tags(GroupName);
                    ep.Description(x => x.WithTags(GroupName)
              );
                });
            }
        }
    }
}

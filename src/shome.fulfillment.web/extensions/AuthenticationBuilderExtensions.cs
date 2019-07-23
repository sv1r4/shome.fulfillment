using System;
using Microsoft.AspNetCore.Authentication;
using shome.fulfillment.web.authentication;

namespace shome.fulfillment.web.extensions
{
    public static class AuthenticationBuilderExtensions
    {  public static AuthenticationBuilder AddFulfillmentCustomAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<FulfillmentAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<FulfillmentAuthenticationOptions, FulfillmentCustomAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}

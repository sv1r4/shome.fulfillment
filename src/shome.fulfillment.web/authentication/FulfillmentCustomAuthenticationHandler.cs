using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace shome.fulfillment.web.authentication
{
    public class FulfillmentCustomAuthenticationHandler : AuthenticationHandler<FulfillmentAuthenticationOptions>
    {

        public FulfillmentCustomAuthenticationHandler(IOptionsMonitor<FulfillmentAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var auth = Context.Request.Headers["Authorization"].ToString();
            
            if (!auth.Equals($"{FulfillmentAuthDefaults.Scheme} {Options.Secret}", StringComparison.Ordinal))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid authorization header value"));
            }
            
            
            var ci = new ClaimsIdentity(Enumerable.Empty<Claim>(), Scheme.Name);
            var cp = new ClaimsPrincipal(ci);

            return Task.FromResult(AuthenticateResult.Success(
                new AuthenticationTicket(
                    cp,
                    this.Scheme.Name)));

        }
    }


    public class FulfillmentAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Secret { get; set; }

    }
    

}

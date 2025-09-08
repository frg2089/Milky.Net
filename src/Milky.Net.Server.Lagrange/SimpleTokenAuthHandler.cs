using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Milky.Net.Server.Lagrange;

public sealed class SimpleTokenAuthHandlerOptions : AuthenticationSchemeOptions
{
    public string? Token { get; set; }
}

public sealed class SimpleTokenAuthHandler(
    IOptionsMonitor<SimpleTokenAuthHandlerOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<SimpleTokenAuthHandlerOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var currentToken = Options.Token;
        if (!string.IsNullOrWhiteSpace(currentToken))
        {
            string? token;
            if (Request.Headers.Authorization.Count is not 0)
            {
                var authHeader = Request.Headers.Authorization.ToString();
                if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    return Task.FromResult(AuthenticateResult.Fail("Invalid token"));

                token = authHeader[7..].Trim();
            }
            else
            {
                token = Request.Query.FirstOrDefault(i => i.Key is "access_token").Value;
            }

            if (token != currentToken)
                return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
        }

        var claims = new[] { new Claim(ClaimTypes.Name, "ConfiguredUser") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

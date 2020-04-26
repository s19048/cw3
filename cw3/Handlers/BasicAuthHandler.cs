using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace cw3.Handlers
{
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthHandler(
           IOptionsMonitor<AuthenticationSchemeOptions> options,
           ILoggerFactory logger,
           UrlEncoder encoder,
           ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing authorization header");


            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialsBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialsBytes).Split(":");

            if (credentials.Length != 2)
                return AuthenticateResult.Fail("Incorrect authorization header value");

            using (var sqlConnection = new SqlConnection("Data Source=db-mssql16.pjwstk.edu.pl; Initial Catalog=s19048;User ID=apbds19048;Password=admin"))
            using (var command = new SqlCommand())
            {
                sqlConnection.Open();
                command.Connection = sqlConnection;
                command.CommandText = "SELECT IndexNumber FROM Student WHERE IndexNumber = @index AND Password = @password";
                command.Parameters.AddWithValue("index", credentials[0]);
                command.Parameters.AddWithValue("password", credentials[1]);
                var dr = command.ExecuteReader();
                if (!dr.Read())
                {
                    return AuthenticateResult.Fail("Login or password is not correct");
                }
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "smith"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "student"),
                new Claim(ClaimTypes.Role, "Employee")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}

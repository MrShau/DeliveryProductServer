using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeliveryProductAPI.Server.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string SECURITY_KEY;

        public const string ISSUER = "EventsManager";
        public const string AUDINCE = "EventsManagerClient";

        public const int LIFETIME = 1440;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            SECURITY_KEY = configuration.GetValue<string>("JWT_KEY") ?? throw new NullReferenceException("JWT KEY IS NULL !");
        }

        public SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SECURITY_KEY));

        public string GenerateToken(string login, string role)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            var token = new JwtSecurityToken(
                issuer: ISSUER,
                audience: AUDINCE,
                notBefore: DateTime.UtcNow,
                claims: claimsIdentity.Claims,
                expires: DateTime.UtcNow.AddMinutes(LIFETIME),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

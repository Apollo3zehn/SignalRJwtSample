using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace SignalRJwtSample
{
    public class UserService
    {
        private JwtSecurityTokenHandler JwtTokenHandler = new JwtSecurityTokenHandler();
        private DatabaseContext _context;

        public UserService(DatabaseContext context)
        {
            _context = context;
        }

        public string Authenticate(string username, string password)
        {
            var isAuthenticated = _context.Users.SingleOrDefault(user => user.Name == username && user.Password == password) != null;

            return isAuthenticated ? this.GenerateToken(username) : null;
        }

        private string GenerateToken(string username)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, username) };
            var credentials = new SigningCredentials(Startup.SecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken("SignalRTestServer", "SignalRTests", claims, expires: DateTime.UtcNow.AddSeconds(30), signingCredentials: credentials);

            return JwtTokenHandler.WriteToken(token);
        }
    }
}

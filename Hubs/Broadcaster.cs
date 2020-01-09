using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalRJwtSample
{
    public class Broadcaster : Hub
    {
        private UserService _userService;

        public Broadcaster(UserService userService)
        {
            _userService = userService;
        }

        public Task<string> GenerateToken(string username, string password)
        {
            var token = _userService.Authenticate(username, password);

            return Task.FromResult(token);
        }

        [Authorize(JwtBearerDefaults.AuthenticationScheme)]
        public Task Broadcast(string message)
        {
            return Clients.All.SendAsync("SendMessage", message);
        }
    }
}

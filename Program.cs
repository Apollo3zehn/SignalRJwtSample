using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace SignalRJwtSample
{
    // https://github.com/dotnet/aspnetcore/tree/master/src/SignalR/samples
    public class Program
    {
        public static void Main(string[] args)
        {
            // replace this by a real database management
            using (var context = new DatabaseContext())
            {
                context.Database.EnsureCreated();

                if (!context.Users.Any(user => user.Name == "User1"))
                {
                    context.Users.Add(new User() { Name = "User1", Password = "SecretPassword" });
                    context.SaveChanges();
                }
            }

            // start web host
            new WebHostBuilder()
                .ConfigureLogging(factory =>
                {
                    factory.AddConsole();
                    factory.AddFilter("Console", level => level >= LogLevel.Information);
                })
                .UseUrls("http://localhost:4000")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build().Run();
        }
    }
}

using Firenet;
using UnitTests.Models;

namespace UnitTests
{
    public class AppDbContext5 : FireContext
    {
        public IFireCollection<User> Users { get; set; }

        protected override void OnConfiguring(FireOption options)
        {
            options.UseGoogleEnvironmentVariable();
            options.EnableWarningLogger();
        }
    }
}

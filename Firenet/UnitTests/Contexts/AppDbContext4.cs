using Firenet;
using UnitTests.Models;

namespace UnitTests
{
    public class AppDbContext4 : FireContext
    {
        public IFireCollection<User> Users { get; set; }

        protected override void OnConfiguring(FireOption options)
        {
            options.EnableWarningLogger();
        }
    }
}

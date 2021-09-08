using Firenet;
using System;
using UnitTests.Models;

namespace UnitTests
{
    public class AppDbContext : FireContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(Action<FireOption> options) : base(options)
        {
        }

        public IFireCollection<User> Users { get; set; }
        public IFireCollection<Post> Posts { get; set; }
    }
}

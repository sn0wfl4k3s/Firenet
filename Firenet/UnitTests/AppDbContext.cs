using Firenet;
using System;
using UnitTests.Models;

namespace UnitTests
{
    public class AppDbContext : FireContext
    {
        protected override string JsonCredentials =>
            $@"C:\Users\{Environment.UserName}\Downloads\firebase-admin.json";

        public IFireCollection<User> Users { get; set; }
        public IFireCollection<Post> Posts { get; set; }
    }
}

using Firenet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTests.Models;

namespace UnitTests
{
    public class FirestoreDatabase : IDisposable
    {
        public AppDbContext Context { get; private set; }

        public FirestoreDatabase()
        {
            Context = FireContextBuilder<AppDbContext>.Build();
        }

        public void LoadAllData()
        {
            var usersThere = Context.Users.ToList();

            if (usersThere.Count() > 0)
                Context.Users.DeleteRange(usersThere.Select(u => u.Id));

            var users = new List<User>
            {
                new User { Name = "Eduardo", Email = "eduardo@gmail.com", IsAdmin = true, Points = 10, Release = DateTime.Now.AddYears(-2) },
                new User { Name = "Ricardo", Email = "ricardo@gmail.com", IsAdmin = false, Points = 20, Release = DateTime.Now.AddYears(-1) },
                new User { Name = "Eduarda", Email = "eduarda@gmail.com", IsAdmin = false, Points = 30, Release = DateTime.Now.AddYears(-3) },
                new User { Name = "Ronaldo", Email = "ronaldo@gmail.com", IsAdmin = false, Points = 40, Release = DateTime.Now.AddYears(-4) },
                new User { Name = "Fabiana", Email = "fabiana@gmail.com", IsAdmin = false, Points = 50, Release = DateTime.Now.AddYears(-5) },
            };

            Context.Users.AddRange(users);
        }

        public void Dispose()
        {
            Context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

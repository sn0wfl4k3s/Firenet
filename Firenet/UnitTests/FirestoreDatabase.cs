using System;
using System.Collections.Generic;
using System.Linq;
using UnitTests.Models;

namespace UnitTests
{
    public class FirestoreDatabase : IDisposable
    {
        public static readonly string CredentialsPath = $@"C:\Users\{Environment.UserName}\Downloads\firebase-admin.json";
        public static readonly string VariableGoogle = "GOOGLE_APPLICATION_CREDENTIALS";

        public AppDbContext Context { get; private set; }

        public FirestoreDatabase()
        {
            Context = new AppDbContext();
        }

        public void LoadAllData()
        {
            var usersThere = Context.Users.ToArray();

            if (usersThere.Length > 0)
                Context.Users.DeleteRange(usersThere.Select(u => u.Id));

            string[] ids = new[] { "asdad", "laosas", "qweuqw", "132456" };

            var users = new List<User>
            {
                new User { Name = "Eduardo", Email = "eduardo@gmail.com", IsAdmin = true, Points = 10, Release = DateTime.Now.AddYears(-2), Ids = ids },
                new User { Name = "Ricardo", Email = "ricardo@gmail.com", IsAdmin = false, Points = 20, Release = DateTime.Now.AddYears(-1) },
                new User { Name = "Eduarda", Email = "eduarda@gmail.com", IsAdmin = false, Points = 30, Release = DateTime.Now.AddYears(-3) },
                new User { Name = "Ronaldo", Email = "ronaldo@gmail.com", IsAdmin = false, Points = 40, Release = DateTime.Now.AddYears(-4) },
                new User { Name = "Fabiana", Email = "fabiana@gmail.com", IsAdmin = false, Points = 50, Release = DateTime.Now.AddYears(-5) },
            };

            foreach (var userToSave in users)
            {
                Context.Users.Add(userToSave);
            }

        }

        public void Dispose()
        {
            Context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

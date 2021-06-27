using System;
using System.Collections.Generic;
using System.Linq;
using UnitTests.Models;
using Xunit;

namespace UnitTests
{
    public class QueryUnitTests : IClassFixture<FirestoreDatabase>
    {
        private readonly AppDbContext _context;
        private IEnumerable<User> query;

        public QueryUnitTests(FirestoreDatabase firestore)
        {
            _context = firestore.Context;
            firestore.LoadAllData();
        }

        [Fact(DisplayName = "Query all")]
        public void Query00()
        {
            query = _context.Users.ToList();
            Assert.True(query.Count() == 5, $"Retornaram {query.Count()} elementos.");

            query = _context.Users.AsQueriable().ToList();
            Assert.True(query.Count() == 5, $"Retornaram {query.Count()} elementos.");
        }

        [Fact(DisplayName = "Query com '=='")]
        public void Query0()
        {
            query = _context.Users.AsQueriable().Where(u => u.Name == "Eduardo" || u.Name == "Ronaldo").ToList();
            Assert.True(query.Count() == 2, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduardo");
            Assert.Contains(query, q => q.Name == "Ronaldo");

            query = _context.Users.AsQueriable().Where(u => u.Name == "Eduardo" && u.Email == "nothing@gmail.com").ToList();
            Assert.True(query.Count() == 0, $"Retornaram {query.Count()} elementos.");
        }

        [Fact(DisplayName = "Query com '!='")]
        public void Query1()
        {
            query = _context.Users.AsQueriable().Where(u => u.Name != "Eduardo").ToList();
            Assert.True(query.Count() == 4, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduarda");
        }

        [Fact(DisplayName = "Query com 'StartsWith'")]
        public void Query2()
        {
            query = _context.Users.AsQueriable().Where(u => u.Name.StartsWith("Edu")).ToList();
            Assert.True(query.Count() == 2, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduardo");
            Assert.Contains(query, q => q.Name == "Eduarda");

            query = _context.Users.AsQueriable().Where(u => u.Name.StartsWith("Rob")).ToList();
            Assert.True(query.Count() == 0, $"Retornaram {query.Count()} elementos.");

            query = _context.Users.AsQueriable().Where(u => u.Name.StartsWith("du")).ToList();
            Assert.True(query.Count() == 0, $"Retornaram {query.Count()} elementos.");

            query = _context.Users.AsQueriable().Where(u => u.Name.StartsWith("ardo")).ToList();
            Assert.True(query.Count() == 0, $"Retornaram {query.Count()} elementos.");
        }

        [Fact(DisplayName = "Query com '>'")]
        public void Query3()
        {
            query = _context.Users.AsQueriable().Where(u => u.Points > 20).ToList();
            Assert.True(query.Count() == 3, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduarda");
            Assert.Contains(query, q => q.Name == "Ronaldo");
            Assert.Contains(query, q => q.Name == "Fabiana");
        }

        [Fact(DisplayName = "Query com '>='")]
        public void Query4()
        {
            query = _context.Users.AsQueriable().Where(u => u.Points >= 20).ToList();
            Assert.True(query.Count() == 4, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Ricardo");
            Assert.Contains(query, q => q.Name == "Eduarda");
            Assert.Contains(query, q => q.Name == "Ronaldo");
            Assert.Contains(query, q => q.Name == "Fabiana");
        }

        [Fact(DisplayName = "Query com '<'")]
        public void Query5()
        {
            query = _context.Users.AsQueriable().Where(u => u.Points < 40).ToList();
            Assert.True(query.Count() == 3, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduardo");
            Assert.Contains(query, q => q.Name == "Ricardo");
            Assert.Contains(query, q => q.Name == "Eduarda");
        }

        [Fact(DisplayName = "Query com '<='")]
        public void Query6()
        {
            query = _context.Users.AsQueriable().Where(u => u.Points <= 40).ToList();
            Assert.True(query.Count() == 4, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduardo");
            Assert.Contains(query, q => q.Name == "Ricardo");
            Assert.Contains(query, q => q.Name == "Eduarda");
            Assert.Contains(query, q => q.Name == "Ronaldo");
        }

        [Fact(DisplayName = "Query com Boolean")]
        public void Query7()
        {
            query = _context.Users.AsQueriable().Where(u => u.IsAdmin).ToList();
            Assert.True(query.Count() == 1, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduardo");

            query = _context.Users.AsQueriable().Where(u => !u.IsAdmin).ToList();
            Assert.True(query.Count() == 4, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Ricardo");
            Assert.Contains(query, q => q.Name == "Eduarda");
            Assert.Contains(query, q => q.Name == "Ronaldo");
            Assert.Contains(query, q => q.Name == "Fabiana");

            query = _context.Users.AsQueriable().Where(u => u.IsAdmin == true).ToList();
            Assert.True(query.Count() == 1, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduardo");

            query = _context.Users.AsQueriable().Where(u => u.IsAdmin == false).ToList();
            Assert.True(query.Count() == 4, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Ricardo");
            Assert.Contains(query, q => q.Name == "Eduarda");
            Assert.Contains(query, q => q.Name == "Ronaldo");
            Assert.Contains(query, q => q.Name == "Fabiana");
        }

        [Fact(DisplayName = "Query composta de boolean e logic")]
        public void Query8()
        {
            query = _context.Users.AsQueriable().Where(u => u.Points > 30 || u.IsAdmin).ToList();
            Assert.True(query.Count() == 3, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduardo");
            Assert.Contains(query, q => q.Name == "Ronaldo");
            Assert.Contains(query, q => q.Name == "Fabiana");

            query = _context.Users.AsQueriable().Where(u => u.Points > 30).OrderBy(u => u.Points).ToList();
            Assert.True(query.Count() == 2, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Ronaldo");
            Assert.Contains(query, q => q.Name == "Fabiana");
        }

        [Fact(DisplayName = "Query com Datetime")]
        public void Query9()
        {
            query = _context.Users.AsQueriable().Where(u => u.Release > DateTime.Now.AddYears(-4)).ToList();
            Assert.True(query.Count() == 3, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduardo");
            Assert.Contains(query, q => q.Name == "Ricardo");
            Assert.Contains(query, q => q.Name == "Eduarda");


            var date = DateTime.Now.AddYears(-4);
            query = _context.Users.AsQueriable().Where(u => u.Release > date).ToList();
            Assert.True(query.Count() == 3, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduardo");
            Assert.Contains(query, q => q.Name == "Ricardo");
            Assert.Contains(query, q => q.Name == "Eduarda");
        }

        [Fact(DisplayName = "Query com OrderBy")]
        public void Query10()
        {
            query = _context.Users.AsQueriable().OrderBy(u => u.Name).ToList();
            Assert.True(query.First().Name == "Eduarda");
            Assert.True(query.Last().Name == "Ronaldo");

            query = _context.Users.AsQueriable().OrderBy(u => u.Points).ToList();
            Assert.True(query.First().Name == "Eduardo");
            Assert.True(query.Last().Name == "Fabiana");

            query = _context.Users.AsQueriable().OrderBy(u => u.Release).ToList();
            Assert.True(query.First().Name == "Fabiana");
            Assert.True(query.Last().Name == "Ricardo");
        }

        [Fact(DisplayName = "Query com OrderByDescending")]
        public void Query11()
        {
            query = _context.Users.AsQueriable().OrderByDescending(u => u.Name).ToList();
            Assert.True(query.First().Name == "Ronaldo");
            Assert.True(query.Last().Name == "Eduarda");

            query = _context.Users.AsQueriable().OrderByDescending(u => u.Points).ToList();
            Assert.True(query.First().Name == "Fabiana");
            Assert.True(query.Last().Name == "Eduardo");

            query = _context.Users.AsQueriable().OrderByDescending(u => u.Release).ToList();
            Assert.True(query.First().Name == "Ricardo");
            Assert.True(query.Last().Name == "Fabiana");
        }

        [Fact(DisplayName = "Query com First")]
        public void Query12()
        {
            var user = _context.Users.AsQueriable().First(u => u.Name == "Eduardo");
            Assert.True(user.Name == "Eduardo");

            user = _context.Users.AsQueriable().First(u => u.Name.StartsWith("Edu"));
            Assert.True(user.Name == "Eduardo");
        }

        [Fact(DisplayName = "Query com Last")]
        public void Query13()
        {
            var user = _context.Users.AsQueriable().Last();
            Assert.True(user.Name == "Fabiana");

            user = _context.Users.AsQueriable().Last(u => u.Name.StartsWith("Edu"));
            Assert.True(user.Name == "Eduarda");
        }
    }
}



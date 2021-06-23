using Firenet;
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
        }

        [Fact(DisplayName = "Query com Datetime")]
        public void Query9()
        {
            query = _context.Users.AsQueriable().Where(u => u.Release > DateTime.Now.AddYears(-4)).ToList();
            Assert.True(query.Count() == 3, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduardo");
            Assert.Contains(query, q => q.Name == "Ricardo");
            Assert.Contains(query, q => q.Name == "Eduarda");
        }
    }
}



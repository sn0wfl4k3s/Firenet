using Firenet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.Models;
using Xunit;

namespace UnitTests
{
    public class UnitTests : IDisposable
    {
        private readonly AppDbContext _context;

        public UnitTests()
        {
            _context = FireContextBuilder<AppDbContext>.Build();
        }

        [Fact]
        public async Task Query()
        {
            var usersThere = await _context.Users.ToListAsync();
            await _context.Users.DeleteRangeAsync(usersThere.Select(u => u.Id));
            var users = new List<User>
            {
                new User { Name = "Eduardo", Email = "eduardo@gmail.com" },
                new User { Name = "Eduarda", Email = "eduarda@gmail.com" },
                new User { Name = "Ronaldo", Email = "ronaldo@gmail.com" },
            };
            var added = await _context.Users.AddRangeAsync(users);

            IEnumerable<User> query;
            
            query = _context.Users.AsQueriable().Where(u => u.Name == "Eduardo" || u.Name == "Ronaldo").ToList();
            Assert.True(query.Count() == 2, $"Retornaram {query.Count()} elementos.");
            Assert.Contains(query, q => q.Name == "Eduardo");
            Assert.Contains(query, q => q.Name == "Ronaldo");

            query = _context.Users.AsQueriable().Where(u => u.Name == "Eduardo" && u.Email == "nothing@gmail.com").ToList();
            Assert.True(query.Count() == 0, $"Retornaram {query.Count()} elementos.");

            //query = _context.Users.AsQueriable().Where(u => u.Name.StartsWith("Edu")).ToList();
            //Assert.True(query.Count() == 2, $"Retornaram {query.Count()} elementos.");
            //Assert.Contains(query, q => q.Name == "Eduardo");
            //Assert.Contains(query, q => q.Name == "Eduarda");
        }

        [Fact(Skip = "funcionando")]
        public async Task Delete_all()
        {
            var users = await _context.Users.ToListAsync();
            await _context.Users.DeleteRangeAsync(users.Select(u => u.Id));
            users = await _context.Users.ToListAsync();
            Assert.True(users.Count() is 0);
        }

        [Theory(Skip = "funcionando")]
        [InlineData("Fulano", "fulano@gmail.com")]
        [InlineData("Maria", "mariazinha@gmail.com")]
        [InlineData("João", "Joãozin@gmail.com")]
        public async Task Add_users(string name, string email)
        {
            var user = new User { Name = name, Email = email };
            var added = await _context.Users.AddAsync(user);
            var review = await _context.Users.FindAsync(added.Id);
            Assert.Equal(added.Id, review.Id);
            Assert.Equal(added.Name, review.Name);
            Assert.Equal(added.Email, review.Email);
            await _context.Users.DeleteAsync(added.Id);
        }

        [Theory(Skip = "funcionando")]
        [InlineData("Joãozin@gmail.com", "jhon@gmail.com")]
        [InlineData("fulano@gmail.com", "fulano2@gmail.com")]
        [InlineData("mariazinha@gmail.com", "mariazinha_nçva@gmail.com")]
        public async Task Update_users(string emailOld, string emailNew)
        {
            var user = new User { Name = "Random", Email = emailOld };
            var added = await _context.Users.AddAsync(user);
            var review = await _context.Users.FindAsync(added.Id);
            Assert.Equal(added.Id, review.Id);
            Assert.Equal(added.Name, review.Name);
            Assert.Equal(added.Email, emailOld);
            review.Email = emailNew;
            var updated = await _context.Users.UpdateAsync(added.Id, review);
            Assert.Equal(updated.Id, review.Id);
            Assert.Equal(updated.Name, review.Name);
            Assert.Equal(updated.Email, emailNew);
            await _context.Users.DeleteAsync(review.Id);
        }

        public void Dispose() => _context.Dispose();
    }
}

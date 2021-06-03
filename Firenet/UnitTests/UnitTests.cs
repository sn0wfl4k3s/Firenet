using Firenet;
using System;
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
        public async Task Delete_all()
        {
            var users = await _context.Users.ToListAsync();
            await _context.Users.DeleteRangeAsync(users.Select(u => u.Id));
            users = await _context.Users.ToListAsync();
            Assert.True(users.Count() is 0);
        }

        [Theory]
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
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using UnitTests.Models;
using Xunit;

namespace UnitTests
{
    public class V101UnitTests : IClassFixture<FirestoreDatabase>
    {
        private readonly AppDbContext _context;

        public V101UnitTests(FirestoreDatabase firestore)
        {
            _context = firestore.Context;
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
            await _context.Users.DeleteAsync(added.Id);
        }

        [Theory]
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
    }
}

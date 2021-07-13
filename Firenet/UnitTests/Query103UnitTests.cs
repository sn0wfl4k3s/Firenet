using Xunit;

namespace UnitTests
{
    public class Query103UnitTests : IClassFixture<FirestoreDatabase>
    {
        private readonly AppDbContext _context;

        public Query103UnitTests(FirestoreDatabase firestore)
        {
            _context = firestore.Context;
            firestore.LoadAllData();
        }

        [Fact(DisplayName = "Query com FirstOrDeafult")]
        public void FirstOrDeafult()
        {
            var admin = _context.Users.AsQueryable().FirstOrDefault(u => u.IsAdmin);
            Assert.NotNull(admin);
            Assert.Equal("Eduardo", admin.Name);

            var userLessThanTen = _context.Users.AsQueryable().FirstOrDefault(u => u.Points < 10);
            Assert.Null(userLessThanTen);

            var userGreaterThanTen = _context.Users.AsQueryable().FirstOrDefault(u => u.Points > 10);
            Assert.NotNull(userGreaterThanTen);
            Assert.Equal("Ricardo", userGreaterThanTen.Name);
        }

        [Fact(DisplayName = "Query com LastOrDeafult")]
        public void LastOrDeafult()
        {
            var maiorQue10 = _context.Users.AsQueryable().LastOrDefault(u => u.Points > 10);
            Assert.NotNull(maiorQue10);
            Assert.Equal("Fabiana", maiorQue10.Name);

            var ultimoNaoAdmin = _context.Users.AsQueryable().LastOrDefault(u => !u.IsAdmin);
            Assert.NotNull(ultimoNaoAdmin);
            Assert.Equal("Fabiana", ultimoNaoAdmin.Name);

            var lessThanTen = _context.Users.AsQueryable().LastOrDefault(u => u.Points < 10);
            Assert.Null(lessThanTen);
        }
    }
}

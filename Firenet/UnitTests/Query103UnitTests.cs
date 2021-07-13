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

        [Fact(DisplayName = "Query com Count")]
        public void Count()
        {
            int moreThanTen = _context.Users.AsQueryable().Where(u => u.Points > 10).Count();
            Assert.Equal(4, moreThanTen);

            moreThanTen = _context.Users.AsQueryable().Count(u => u.Points > 10);
            Assert.Equal(4, moreThanTen);

            int nameEduardoOrLessThan40 = _context.Users.AsQueryable().Where(u => u.Name == "Eduardo" || u.Points < 40).Count();
            Assert.Equal(3, nameEduardoOrLessThan40);

            nameEduardoOrLessThan40 = _context.Users.AsQueryable().Count(u => u.Name == "Eduardo" || u.Points < 40);
            Assert.Equal(3, nameEduardoOrLessThan40);
        }
    }
}

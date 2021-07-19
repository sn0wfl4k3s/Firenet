using Xunit;

namespace UnitTests
{
    public class Query104UnitTests : IClassFixture<FirestoreDatabase>
    {
        private readonly AppDbContext _context;

        public Query104UnitTests(FirestoreDatabase firestore)
        {
            _context = firestore.Context;
            firestore.LoadAllData();
        }

        [Fact(DisplayName = "Query com Any")]
        public void AnyWithoutExpressionInAssigment()
        {
            bool temEdu = _context.Users.AsQueryable().Where(u => u.Name == "Eduardo").Any();
            Assert.True(temEdu);
        }

        [Fact(DisplayName = "Query com All")]
        public void AllQuery()
        {
            bool todosSaoAdmin = _context.Users.AsQueryable().All(u => u.IsAdmin);
            Assert.False(todosSaoAdmin);

            todosSaoAdmin = _context.Users.AsQueryable().Where(u => u.IsAdmin).All();
            Assert.False(todosSaoAdmin);

            bool todosSaoAbaixoDe60 = _context.Users.AsQueryable().All(u => u.Points < 60);
            Assert.True(todosSaoAbaixoDe60);

            todosSaoAbaixoDe60 = _context.Users.AsQueryable().Where(u => u.Points < 60).All();
            Assert.True(todosSaoAbaixoDe60);
        }
    }
}

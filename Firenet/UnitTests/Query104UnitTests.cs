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
    }
}

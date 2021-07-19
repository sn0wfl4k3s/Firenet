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

        [Fact(DisplayName = "Query com LongCount")]
        public void QueryLongCount()
        {
            long quantidadeDeUsuarios = _context.Users.AsQueryable().Count();
            Assert.Equal(5, quantidadeDeUsuarios);

            quantidadeDeUsuarios = _context.Users.AsQueryable().Where(u => true).Count();
            Assert.Equal(5, quantidadeDeUsuarios);

            quantidadeDeUsuarios = _context.Users.AsQueryable().Where(u => false).Count();
            Assert.Equal(0, quantidadeDeUsuarios);

            long quantidadeMaiorQue20 = _context.Users.AsQueryable().Where(u => u.Points > 20).Count();
            Assert.Equal(3, quantidadeMaiorQue20);

            quantidadeMaiorQue20 = _context.Users.AsQueryable().Count(u => u.Points > 20);
            Assert.Equal(3, quantidadeMaiorQue20);
        }
    }
}

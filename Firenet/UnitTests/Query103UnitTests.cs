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

        [Fact(DisplayName = "Query com Select")]
        public void Select()
        {
            int[] userPoints = _context.Users.AsQueryable().Select(u => u.Points).ToArray();
            Assert.Equal(5, userPoints.Length);
            Assert.Equal(10, userPoints[0]);
            Assert.Equal(20, userPoints[1]);
            Assert.Equal(30, userPoints[2]);
            Assert.Equal(40, userPoints[3]);
            Assert.Equal(50, userPoints[4]);

            int[] userPoints2 = _context.Users.AsQueryable().Select(u => u.Points + 2).ToArray();
            Assert.Equal(5, userPoints2.Length);
            Assert.Equal(12, userPoints2[0]);
            Assert.Equal(22, userPoints2[1]);
            Assert.Equal(32, userPoints2[2]);
            Assert.Equal(42, userPoints2[3]);
            Assert.Equal(52, userPoints2[4]);

            string[] userNames = _context.Users.AsQueryable().Select(u => u.Name).ToArray();
            Assert.Equal(5, userNames.Length);
            Assert.Equal("Eduardo", userNames[0]);
            Assert.Equal("Ricardo", userNames[1]);
            Assert.Equal("Eduarda", userNames[2]);
            Assert.Equal("Ronaldo", userNames[3]);
            Assert.Equal("Fabiana", userNames[4]);

            string[] userNames2 = _context.Users.AsQueryable().Select(u => u.Name + "?").ToArray();
            Assert.Equal(5, userNames2.Length);
            Assert.Equal("Eduardo?", userNames2[0]);
            Assert.Equal("Ricardo?", userNames2[1]);
            Assert.Equal("Eduarda?", userNames2[2]);
            Assert.Equal("Ronaldo?", userNames2[3]);
            Assert.Equal("Fabiana?", userNames2[4]);

            string[] userNameAndEmail = _context.Users.AsQueryable().Select(u => u.Name + ": " + u.Email).ToArray();
            Assert.Equal(5, userNameAndEmail.Length);
            Assert.Equal("Eduardo: eduardo@gmail.com", userNameAndEmail[0]);

            int[] moreSelects = _context.Users.AsQueryable().Select(u => u.Points + 2).Select(p => p * 2).ToArray();
            Assert.Equal(5, moreSelects.Length);
            Assert.Equal(24, moreSelects[0]);
            Assert.Equal(44, moreSelects[1]);
            Assert.Equal(64, moreSelects[2]);
            Assert.Equal(84, moreSelects[3]);
            Assert.Equal(104, moreSelects[4]);
        }
    }
}

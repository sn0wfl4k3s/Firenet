using Firenet;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class Features111 : IClassFixture<FirestoreDatabase>
    {
        public readonly ITestOutputHelper _output;
        public readonly FirestoreDatabase _firestore;

        public Features111(FirestoreDatabase firestore, ITestOutputHelper output)
        {
            _output = output;
            _firestore = firestore;
            _firestore.LoadAllData();
        }

        [Fact(DisplayName = "Query com logger")]
        public void WarningLog()
        {
            using var context = new AppDbContext(options => options
                    .SetJsonCredentialsPath(FirestoreDatabase.CredentialsPath)
                    .EnableWarningLogger(_output.WriteLine));

            Assert.Throws<AggregateException>(() =>
            {
                var data = context.Users
                    .AsQueryable()
                    .Where(u => u.Points > 30)
                    .OrderBy(u => u.Name) // ← it's to cause problem
                    .ToArray();
            });
        }

        [Fact(DisplayName = "Converter registry")]
        public void ConverterRegistry()
        {
            using var context = new AppDbContext2(options => options
                    .SetJsonCredentialsPath(FirestoreDatabase.CredentialsPath)
                    .EnableWarningLogger(_output.WriteLine));

            var ids = context.Users.AsQueryable().Select(u => u.Id).ToArray();
            context.Users.DeleteRange(ids);

            var user1 = new User2 { Hash = Guid.NewGuid(), Type = Type.Admin };
            var user2 = new User2 { Hash = Guid.NewGuid(), Type = Type.Default };
            context.Users.Add(user1);
            var users = context.Users.AsQueryable().ToArray();
            Assert.Single(users);
            context.Users.Add(user2);
            users = context.Users.AsQueryable().ToArray();
            Assert.Equal(2, users.Length);
        }

        [Fact(DisplayName = "Select with Document Id")]
        public void SelectDocumentId ()
        {
            using var context = new AppDbContext(options => options
                    .SetJsonCredentialsPath(FirestoreDatabase.CredentialsPath)
                    .EnableWarningLogger(_output.WriteLine));

            string[] userids = context.Users.AsQueryable().Select(u => u.Id).ToArray();
            Assert.NotNull(userids[0]);
            Assert.NotNull(userids[1]);
            Assert.NotNull(userids[2]);
            Assert.NotNull(userids[3]);
            Assert.NotNull(userids[4]);
        }

        [Fact(DisplayName = "Select with Created")]
        public void SelectCreated()
        {
            using var context = new AppDbContext(options => options
                    .SetJsonCredentialsPath(FirestoreDatabase.CredentialsPath)
                    .EnableWarningLogger(_output.WriteLine));


            int[] userids = context.Users.AsQueryable().Select(u => u.Release.Value.Year).ToArray();
            Assert.Contains(2020, userids);
            Assert.Contains(2019, userids);
            Assert.Contains(2018, userids);
            Assert.Contains(2017, userids);
            Assert.Contains(2016, userids);
        }

        [Fact(DisplayName = "teste de instanciação construtor")]
        public void SelectCreated2()
        {
            using var context = new AppDbContext(options => options
                    .SetJsonCredentialsPath(FirestoreDatabase.CredentialsPath)
                    .EnableWarningLogger(_output.WriteLine));

            int[] userids = context.Users.AsQueryable().Select(u => u.Release.Value.Year).ToArray();
            Assert.Contains(2020, userids);
            Assert.Contains(2019, userids);
            Assert.Contains(2018, userids);
            Assert.Contains(2017, userids);
            Assert.Contains(2016, userids);
        }
    }
}

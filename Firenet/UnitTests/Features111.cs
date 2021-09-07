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

        public Features111(FirestoreDatabase firestore, ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Query com logger")]
        public void WarningLog()
        {
            using var context = FireContextBuilder<AppDbContext>
                .Build(options => options
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
            using var context = FireContextBuilder<AppDbContext2>
                .Build(options => options
                    .SetJsonCredentialsPath(FirestoreDatabase.CredentialsPath)
                    //.AddConverter(new GuidConverter())
                    .EnableWarningLogger(_output.WriteLine));

            context.Users.DeleteRange(context.Users.ToArray().Select(u => u.Id));

            var user1 = new User2 { Hash = Guid.NewGuid(), Type = Type.Admin };
            var user2 = new User2 { Hash = Guid.NewGuid(), Type = Type.Default };
            context.Users.Add(user1);
            var users = context.Users.AsQueryable().ToArray();
            Assert.Single(users);
            context.Users.Add(user2);
            users = context.Users.AsQueryable().ToArray();
            Assert.Equal(2, users.Length);
        }
    }
}

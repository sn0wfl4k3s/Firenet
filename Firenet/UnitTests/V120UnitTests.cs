using System;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class V120UnitTests
    {
        public readonly ITestOutputHelper _output;

        public V120UnitTests(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact(DisplayName = "3.1 - without onConf and has no varible")]
        public void ErrorInstantiate()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using var context = new AppDbContext3();
            });
        }

        [Fact(DisplayName = "3.2 - without onConf but has varible")]
        public void Instantiate()
        {
            Environment.SetEnvironmentVariable(FirestoreDatabase.VariableGoogle, FirestoreDatabase.CredentialsPath);
            using var context = new AppDbContext3();
            Environment.SetEnvironmentVariable(FirestoreDatabase.VariableGoogle, string.Empty);
            Assert.NotNull(context);
        }

        [Fact(DisplayName = "4.1 - enableWarning only without variable")]
        public void ErrorInstantiate2()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using var context = new AppDbContext4();
            });
        }

        [Fact(DisplayName = "4.2 - enableWarning only with variable")]
        public void ErrorInstantiate22()
        {
            Environment.SetEnvironmentVariable(FirestoreDatabase.VariableGoogle, FirestoreDatabase.CredentialsPath);
            Assert.Throws<ArgumentNullException>(() =>
            {
                using var context = new AppDbContext4();
            });
            Environment.SetEnvironmentVariable(FirestoreDatabase.VariableGoogle, string.Empty);
        }

        [Fact(DisplayName = "5 - getFromGoogle defined")]
        public void ErrorInstantiate3()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using var context = new AppDbContext5();
            });

            Environment.SetEnvironmentVariable(FirestoreDatabase.VariableGoogle, FirestoreDatabase.CredentialsPath);
            using var context = new AppDbContext5();
            Environment.SetEnvironmentVariable(FirestoreDatabase.VariableGoogle, string.Empty);
            Assert.NotNull(context);
        }

    }
}

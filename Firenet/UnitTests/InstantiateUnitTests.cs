using Firenet;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace UnitTests
{
    public class InstantiateUnitTests
    {
        [Fact(DisplayName = "Instantiation test with environment variable.")]
        public void TestInstantiate()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using var context = new AppDbContext();
            });

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", FirestoreDatabase.CredentialsPath);

            using var context = new AppDbContext();

            Assert.NotNull(context);
        }

        [Fact(DisplayName = "Instantiation test with services and environment variable.")]
        public void TestInstantiateServices()
        {
            var services = new ServiceCollection();

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "");

            Assert.Throws<ArgumentNullException>(() =>
            {
                services.AddFirenet<AppDbContext>();
            });

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", FirestoreDatabase.CredentialsPath);

            services.AddFirenet<AppDbContext>();

            var provider = services.BuildServiceProvider();

            using var context = provider.GetService<AppDbContext>();

            Assert.NotNull(context);
        }
    }
}

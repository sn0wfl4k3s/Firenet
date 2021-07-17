using Firenet;
using Xunit;

namespace UnitTests
{
    public class InstantiateUnitTests
    {
        [Fact(DisplayName = "Teste de instanciação")]
        public void TestInstantiate()
        {
            var context = FireContextBuilder<AppDbContext>.Build();
        }
    }
}

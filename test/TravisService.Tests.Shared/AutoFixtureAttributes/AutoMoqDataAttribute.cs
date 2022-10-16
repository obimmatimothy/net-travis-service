using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace TravisService.Tests.Shared.AutoFixtureAttributes
{
    public sealed class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute() : base(() => CreateFixture())
        {
        }

        private static IFixture CreateFixture() => new Fixture()
            .Customize(new AutoMoqCustomization { ConfigureMembers = true });
    }

}

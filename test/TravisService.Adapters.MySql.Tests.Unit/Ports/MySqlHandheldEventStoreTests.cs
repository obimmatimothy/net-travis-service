using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using TravisService.Adapters.MySql.Ports;
using Xunit;

namespace TravisService.Adapters.MySql.Tests.Unit.Ports
{
    public class MySqlHandheldEventStoreTests
    {

        [Theory, AutoData]
        internal void ConstructorIsGuardedAgainstNullValues(GuardClauseAssertion assertion) =>
            assertion.Verify(typeof(MySqlHandheldEventStore).GetConstructors());
    }
}

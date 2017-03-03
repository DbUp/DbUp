using NUnit.Framework;

namespace DbUp.Tests.TestInfrastructure
{
    [TestFixture]
    public abstract class SpecificationFor<T>
    {
        public T Subject;

        public abstract T Given();
        public abstract void When();

        [SetUp]
        public void SetUp()
        {
            Subject = Given();
            When();
        }
    }
}
namespace DbUp.Tests.TestInfrastructure
{
    public abstract class SpecificationFor<T>
    {
        public T Subject;

        public abstract T Given();
        protected abstract void When();

        public SpecificationFor()
        {
            Subject = Given();
            When();
        }
    }
}
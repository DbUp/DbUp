using System;
using System.Data;
using DbUp.Engine;
using NSubstitute;

namespace DbUp.Specification
{
    public class TestConnectionManager : IConnectionManager
    {
        private readonly IDbConnection connection;

        public TestConnectionManager(IDbConnection connection = null)
        {
            this.connection = connection ?? Substitute.For<IDbConnection>();
        }

        public void RunWithManagedConnection(Action<IDbConnection> action)
        {
            action(connection);
        }

        public T RunWithManagedConnection<T>(Func<IDbConnection, T> actionWithResult)
        {
            return actionWithResult(connection);
        }
    }
}
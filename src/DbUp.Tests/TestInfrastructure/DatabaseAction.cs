using System.Data;

namespace DbUp.Tests.TestInfrastructure
{
    internal class DatabaseAction
    {
        private readonly string action;

        private DatabaseAction(string action)
        {
            this.action = action;
        }

        public static DatabaseAction BeginTransaction()
        {
            return new DatabaseAction("Begin transaction");
        }

        public static DatabaseAction BeginTransaction(IsolationLevel il)
        {
            return new DatabaseAction(string.Format("Begin transaction with isolationLevel of {0}", il));
        }

        public static DatabaseAction CommitTransaction()
        {
            return new DatabaseAction("Commit transaction");
        }

        public static DatabaseAction OpenConnection()
        {
            return new DatabaseAction("Open connection");
        }

        public static DatabaseAction CloseConnection()
        {
            return new DatabaseAction("Close connection");
        }

        public static DatabaseAction DisposeConnection()
        {
            return new DatabaseAction("Dispose connection");
        }

        public static DatabaseAction ExecuteScalarCommand(string commandText)
        {
            return new DatabaseAction(string.Format("Execute scalar command: {0}", commandText));
        }

        public static DatabaseAction ExecuteReaderCommand(string commandText)
        {
            return new DatabaseAction(string.Format("Execute reader command: {0}", commandText));
        }

        public static DatabaseAction ExecuteNonQuery(string commandText)
        {
            return new DatabaseAction(string.Format("Execute non query command: {0}", commandText));
        }

        public static DatabaseAction RollbackTransaction()
        {
            return new DatabaseAction("Rollback transaction");
        }

        public static DatabaseAction DisposeCommand()
        {
            return new DatabaseAction("Dispose command");
        }

        public static DatabaseAction CreateParameter()
        {
            return new DatabaseAction("Create parameter");
        }

        public static DatabaseAction DisposeTransaction()
        {
            return new DatabaseAction("Dispose transaction");
        }

        public static DatabaseAction AddParameterToCommand(object value)
        {
            return new DatabaseAction(string.Format("Add parameter to command: {0}", value));
        }

        protected bool Equals(DatabaseAction other)
        {
            return string.Equals(action, other.action);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DatabaseAction) obj);
        }

        public override int GetHashCode()
        {
            return (action != null ? action.GetHashCode() : 0);
        }

        public static bool operator ==(DatabaseAction left, DatabaseAction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DatabaseAction left, DatabaseAction right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return action;
        }
    }
}
using System;
using System.Runtime.Serialization;

namespace DbUp.SqlServer
{
    [Serializable]
    class TSqlParseException : Exception
    {
        public TSqlParseException()
        { }

        public TSqlParseException(string message)
            : base(message)
        { }

        public TSqlParseException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected TSqlParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}

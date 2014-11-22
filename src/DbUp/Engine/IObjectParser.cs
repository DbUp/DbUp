using System;
using DbUp.Support;

namespace DbUp.Engine
{
    public interface IObjectParser
    {
        string QuoteSqlObjectName(string objectName);
        string QuoteSqlObjectName(string objectName, ObjectNameOptions objectNameOptions);
    }
}

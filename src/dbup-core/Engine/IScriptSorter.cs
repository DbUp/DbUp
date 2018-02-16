using System.Collections.Generic;

namespace DbUp.Engine
{
    public interface IScriptSorter
    {
        IEnumerable<SqlScript> Sort(IEnumerable<SqlScript> filtered);
    }
}
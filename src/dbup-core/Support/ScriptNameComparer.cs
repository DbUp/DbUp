using System.Collections.Generic;

namespace DbUp.Support
{
    public class ScriptNameComparer : IComparer<string>, IEqualityComparer<string>
    {
        private readonly IComparer<string> comparer;

        public ScriptNameComparer(IComparer<string> comparer)
        {
            this.comparer = comparer;
        }

        public int Compare(string x, string y) => comparer.Compare(x, y);

        public bool Equals(string x, string y) => comparer.Compare(x, y) == 0;

        public int GetHashCode(string obj) => obj.GetHashCode();
    }
}
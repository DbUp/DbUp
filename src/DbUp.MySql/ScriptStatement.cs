using System;

namespace DbUp.MySql
{
    internal struct ScriptStatement
    {
        public string Text;
        public int Line;
        public int Position;
    }
}
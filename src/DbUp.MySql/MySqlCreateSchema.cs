using System;
using DbUp.Engine;

namespace DbUp.MySql
{
    public class MySqlCreateSchema : ICreateSchema
    {
        public string Command(string schema)
        {
            return string.Format(@"CREATE DATABASE IF NOT EXISTS {0};", schema);
        }
    }
}

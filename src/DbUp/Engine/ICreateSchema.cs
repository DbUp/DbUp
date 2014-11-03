using System;

namespace DbUp.Engine
{
    public interface ICreateSchema
    {
        string Command(string schema);
    }
}

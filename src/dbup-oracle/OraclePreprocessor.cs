﻿using System;
using System.Collections.Generic;
using System.Text;
using DbUp.Engine;

namespace DbUp.Oracle
{
    public class OraclePreprocessor : IScriptPreprocessor
    {
        public string Process(string contents)
        {
            return contents;
        }
    }
}

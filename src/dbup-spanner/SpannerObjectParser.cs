using System;
using System.Collections.Generic;
using System.Text;
using DbUp.Support;

namespace DbUp.Spanner
{
    /// <summary>
    /// Parses Sql Objects and performs quoting functions
    /// </summary>
    public class SpannerObjectParser : SqlObjectParser
    {
        public SpannerObjectParser() : base("`", "`")
        {
        }
    }
}

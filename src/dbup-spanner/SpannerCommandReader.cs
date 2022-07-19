using System;
using DbUp.Support;

namespace DbUp.Spanner
{
    public class SpannerCommandReader : SqlCommandReader
    {
        /// <summary>
        /// Creates an instance of SpannerCommandReader
        /// </summary>
        public SpannerCommandReader(string sqlText) : base(sqlText, ";", delimiterRequiresWhitespace: false)
        {
        }
    }
}

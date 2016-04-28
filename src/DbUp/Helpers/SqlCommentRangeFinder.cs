using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;

namespace DbUp.Helpers
{
    public static class SqlCommentRangeFinder
    {
        private static readonly char NonMatching = 'a';

        public static List<KeyValuePair<int, int>> FindRanges(string str)
        {
            var results = new List<KeyValuePair<int, int>>();

            var curStartIdx = 0;
            var inBlockComment = false;
            var inLineComment = false;
            var depth = 0;

            for (int i = 0; i < str.Length; i++)
            {
                var curChar = str[i];
                var nextChar = str.Length > i + 1 ? str[i + 1] : NonMatching;
                var prevChar = i > 0 ? str[i - 1] : NonMatching;

                if (curChar == '/' && nextChar == '*'
                    && !inLineComment) // line comments make /* be ignored (like here)
                {
                    depth++;
                    if (!inBlockComment)
                    {
                        inBlockComment = true;
                        curStartIdx = i;
                    }

                }
                else if (prevChar == '*' && curChar == '/'
                    && inBlockComment
                    && i > curStartIdx + 2
                    && !inLineComment) // line comments make /*/*/*/ of any level of nesting be ignored (like here)
                {

                    depth--;
                    if (depth == 0)
                    {
                        inBlockComment = false;
                        results.Add(new KeyValuePair<int, int>(curStartIdx, i));
                    }
                }
                else if (curChar == '-' && nextChar == '-' && !inBlockComment && !inLineComment)
                {
                    inLineComment = true;
                    curStartIdx = i;
                }
                else if (curChar == '\n' && inLineComment)
                {
                    inLineComment = false;
                    results.Add(new KeyValuePair<int, int>(curStartIdx, i));
                }
            }

            // -- in the last line or /* left unclosed
            if (inLineComment || inBlockComment)
            {
                results.Add(new KeyValuePair<int, int>(curStartIdx, str.Length-1));
            }
            return results;
        }
    }
}

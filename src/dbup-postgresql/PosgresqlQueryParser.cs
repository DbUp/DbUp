#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DbUp.Postgresql;

internal static class PostgresqlQueryParser
{
    public static IReadOnlyCollection<string> ParseRawQuery(
        string sql,
        bool standardConformingStrings = true)
    {
        List<string> result = new();
        StringBuilder currentStatementBuilder = new();

        currentStatementBuilder.Clear();

        var currCharOfs = 0;
        var end = sql.Length;
        var ch = '\0';
        int dollarTagStart;
        int dollarTagEnd;
        var currTokenBeg = 0;
        var blockCommentLevel = 0;
        var parenthesisLevel = 0;

        None:
        if (currCharOfs >= end)
            goto Finish;
        var lastChar = ch;
        ch = sql[currCharOfs++];
        NoneContinue:
        while (true)
        {
            switch (ch)
            {
                case '/':
                    goto BlockCommentBegin;
                case '-':
                    goto LineCommentBegin;
                case '\'':
                    if (standardConformingStrings)
                        goto Quoted;
                    goto Escaped;
                case '$':
                    if (!IsIdentifier(lastChar))
                        goto DollarQuotedStart;
                    break;
                case '"':
                    goto Quoted;
                case ';':
                    if (parenthesisLevel == 0)
                        goto SemiColon;
                    break;
                case '(':
                    parenthesisLevel++;
                    break;
                case ')':
                    parenthesisLevel--;
                    break;
                case 'e':
                case 'E':
                    if (!IsLetter(lastChar))
                        goto EscapedStart;
                    break;
            }

            if (currCharOfs >= end)
                goto Finish;

            lastChar = ch;
            ch = sql[currCharOfs++];
        }

        Quoted:
        Debug.Assert(ch is '\'' or '"');
        while (currCharOfs < end && sql[currCharOfs] != ch)
        {
            currCharOfs++;
        }

        if (currCharOfs < end)
        {
            currCharOfs++;
            ch = '\0';
            goto None;
        }

        goto Finish;

        EscapedStart:
        if (currCharOfs < end)
        {
            lastChar = ch;
            ch = sql[currCharOfs++];
            if (ch == '\'')
                goto Escaped;
            goto NoneContinue;
        }

        goto Finish;

        Escaped:
        while (currCharOfs < end)
        {
            ch = sql[currCharOfs++];
            switch (ch)
            {
                case '\'':
                    goto MaybeConcatenatedEscaped;
                case '\\':
                {
                    if (currCharOfs >= end)
                        goto Finish;
                    currCharOfs++;
                    break;
                }
            }
        }

        goto Finish;

        MaybeConcatenatedEscaped:
        while (currCharOfs < end)
        {
            ch = sql[currCharOfs++];
            switch (ch)
            {
                case '\r':
                case '\n':
                    goto MaybeConcatenatedEscaped2;
                case ' ':
                case '\t':
                case '\f':
                    continue;
                default:
                    lastChar = '\0';
                    goto NoneContinue;
            }
        }

        goto Finish;

        MaybeConcatenatedEscaped2:
        while (currCharOfs < end)
        {
            ch = sql[currCharOfs++];
            switch (ch)
            {
                case '\'':
                    goto Escaped;
                case '-':
                {
                    if (currCharOfs >= end)
                        goto Finish;
                    ch = sql[currCharOfs++];
                    if (ch == '-')
                        goto MaybeConcatenatedEscapeAfterComment;
                    lastChar = '\0';
                    goto NoneContinue;
                }
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                case '\f':
                    continue;
                default:
                    lastChar = '\0';
                    goto NoneContinue;
            }
        }

        goto Finish;

        MaybeConcatenatedEscapeAfterComment:
        while (currCharOfs < end)
        {
            ch = sql[currCharOfs++];
            if (ch is '\r' or '\n')
                goto MaybeConcatenatedEscaped2;
        }

        goto Finish;

        DollarQuotedStart:
        if (currCharOfs < end)
        {
            ch = sql[currCharOfs];
            if (ch == '$')
            {
                // Empty tag
                dollarTagStart = dollarTagEnd = currCharOfs;
                goto DollarQuoted;
            }

            if (IsIdentifierStart(ch))
            {
                dollarTagStart = currCharOfs;
                currCharOfs++;
                goto DollarQuotedInFirstDelim;
            }

            lastChar = '$';
            currCharOfs++;
            goto NoneContinue;
        }

        goto Finish;

        DollarQuotedInFirstDelim:
        while (currCharOfs < end)
        {
            lastChar = ch;
            ch = sql[currCharOfs++];
            if (ch == '$')
            {
                dollarTagEnd = currCharOfs - 1;
                goto DollarQuoted;
            }

            if (!IsDollarTagIdentifier(ch))
                goto NoneContinue;
        }

        goto Finish;

        DollarQuoted:
        var tag = sql.Substring(dollarTagStart - 1, dollarTagEnd - dollarTagStart + 2);
        var pos = sql.IndexOf(tag, dollarTagEnd + 1, StringComparison.Ordinal);
        if (pos == -1)
        {
            goto Finish;
        }

        pos += dollarTagEnd + 1; // If the substring is found adjust the position to be relative to the entire string
        currCharOfs = pos + dollarTagEnd - dollarTagStart + 2;
        ch = '\0';
        goto None;

        LineCommentBegin:
        if (currCharOfs < end)
        {
            ch = sql[currCharOfs++];
            if (ch == '-')
                goto LineComment;
            lastChar = '\0';
            goto NoneContinue;
        }

        goto Finish;

        LineComment:
        while (currCharOfs < end)
        {
            ch = sql[currCharOfs++];
            if (ch is '\r' or '\n')
                goto None;
        }

        goto Finish;

        BlockCommentBegin:
        while (currCharOfs < end)
        {
            ch = sql[currCharOfs++];
            switch (ch)
            {
                case '*':
                    blockCommentLevel++;
                    goto BlockComment;
                case '/':
                    continue;
            }

            if (blockCommentLevel > 0)
                goto BlockComment;
            lastChar = '\0';
            goto NoneContinue;
        }

        goto Finish;

        BlockComment:
        while (currCharOfs < end)
        {
            ch = sql[currCharOfs++];
            switch (ch)
            {
                case '*':
                    goto BlockCommentEnd;
                case '/':
                    goto BlockCommentBegin;
            }
        }

        goto Finish;

        BlockCommentEnd:
        while (currCharOfs < end)
        {
            ch = sql[currCharOfs++];
            if (ch == '/')
            {
                if (--blockCommentLevel > 0)
                    goto BlockComment;
                goto None;
            }

            if (ch != '*')
                goto BlockComment;
        }

        goto Finish;

        SemiColon:
        currentStatementBuilder.Append(sql, currTokenBeg, currCharOfs - currTokenBeg - 1);
        result.Add(currentStatementBuilder.ToString());
        while (currCharOfs < end)
        {
            ch = sql[currCharOfs];
            if (char.IsWhiteSpace(ch))
            {
                currCharOfs++;
                continue;
            }

            // TODO: Handle end of line comment? Although psql doesn't seem to handle them...
            currentStatementBuilder.Clear();

            currTokenBeg = currCharOfs;
            goto None;
        }

        return result;

        Finish:
        currentStatementBuilder.Append(sql, currTokenBeg, end - currTokenBeg);
        result.Add(currentStatementBuilder.ToString());
        return result;
    }

    // Is ASCII letter comparison optimization https://github.com/dotnet/runtime/blob/60cfaec2e6cffeb9a006bec4b8908ffcf71ac5b4/src/libraries/System.Private.CoreLib/src/System/Char.cs#L236

    private static bool IsLetter(char ch)
        // [a-zA-Z]
        => (uint)((ch | 0x20) - 'a') <= ('z' - 'a');

    private static bool IsIdentifierStart(char ch)
        // [a-zA-Z_\x80-\xFF]
        => (uint)((ch | 0x20) - 'a') <= ('z' - 'a') || ch == '_' || (uint)(ch - 128) <= 127u;

    private static bool IsDollarTagIdentifier(char ch)
        // [a-zA-Z0-9_\x80-\xFF]
        => (uint)((ch | 0x20) - 'a') <= ('z' - 'a') || (uint)(ch - '0') <= ('9' - '0') || ch == '_' ||
           (uint)(ch - 128) <= 127u;

    private static bool IsIdentifier(char ch)
        // [a-zA-Z0-9_$\x80-\xFF]
        => (uint)((ch | 0x20) - 'a') <= ('z' - 'a') || (uint)(ch - '0') <= ('9' - '0') || ch == '_' || ch == '$' ||
           (uint)(ch - 128) <= 127u;
}

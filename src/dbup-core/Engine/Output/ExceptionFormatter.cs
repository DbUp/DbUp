using System;
using System.Text;

namespace DbUp.Engine.Output
{
    public class ExceptionFormatter
    {
        public static string Format(Exception ex)
            => GetMessageTree(ex);

        public static string Format(AggregateException ex)
            => GetMessageTree(ex);

        static string GetMessageTree(Exception ex, string indent = "")
        {
            if (ex == null) return string.Empty;

            if (ex is AggregateException aggregate)
                return GetMessageTree(aggregate, indent);

            return string.Format("{0}{1}{3}{2}",
                indent,
                ex.Message.Replace(Environment.NewLine, ": "),
                GetMessageTree(ex.InnerException, $"{indent}    "),
                Environment.NewLine);
        }

        static string GetMessageTree(AggregateException ex, string indent = "")
        {
            if (ex == null) return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{indent}{ex.Message} ->");

            foreach (Exception childEx in ex.InnerExceptions)
            {
                builder
                    .AppendLine(
                        GetMessageTree(childEx, $"{indent}    ").TrimEnd(Environment.NewLine.ToCharArray())
                    );
            }

            return builder.ToString();
        }
    }
}

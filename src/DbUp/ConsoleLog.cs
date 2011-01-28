using System;

namespace DbUp
{
    public class ConsoleLog : ILog
    {
        public void WriteInformation(string format, params object[] args)
        {
            Write(ConsoleColor.White, format, args);
        }

        public void WriteError(string format, params object[] args)
        {
            Write(ConsoleColor.Red, format, args);
        }

        public void WriteWarning(string format, params object[] args)
        {
            Write(ConsoleColor.Yellow, format, args);
        }

        private static void Write(ConsoleColor color, string format, object[] args)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(format, args);
            Console.ResetColor();
        }
    }
}
using System.Text.RegularExpressions;

namespace DbUp.Tests.TestInfrastructure
{
    public static class Scrubbers
    {
        public static string ScrubDates(string arg)
        {
            return Regex.Replace(arg, @"\d?\d/\d?\d/\d?\d?\d\d \d?\d:\d\d:\d\d", "<date>");
        }
    }
}
using System.Text.RegularExpressions;
using DbUp.Engine;

namespace Odin.Database.DbUp.PreProcessors
{
    public class ErrorHandlingPreProcessor : IScriptPreprocessor
    {
        private const string PATTERN = @"^GO(\r\n)?";
        private static Regex _regex = new Regex(PATTERN, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        public string Process(string contents)
        {
            contents = _regex.Replace(contents,@"GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
");

            contents += @"
GO
DECLARE @Success AS BIT
SET @Success = 1
SET NOEXEC OFF
IF (@Success = 1) PRINT 'The database update succeeded'
ELSE BEGIN
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
    PRINT 'The database update failed'
END
GO
";
            return contents;
        }
    }
}
using CommandLine;

namespace PostgresSampleApp
{
    public class Options
    {
        [Option(
            'c', 
            "connectionString", 
            Required = true,
            HelpText = "The connection string for the target database.")]
        public string ConnectionString { get; set; }

        [Option(
            'x', 
            "recreate", 
            HelpText = "Indicates the database should be recreated.",
            SetName = "Recreate")]
        public bool RecreateDatabase { get; set; }
        
        [Option(
            'd',
            "dryRun",
            HelpText = "Indicates the changes should not be made to the database. " +
                       "If requested, the HTML report will still be generated.",
            SetName = "Report"
            )]
        public bool DryRun { get; set; }
        
        [Option(
            'r',
            "report",
            Default = false,
            Required = false,
            HelpText = "Generate an HTML upgrade report.",
            SetName = "Report")]
        public bool UpgradeReport { get; set; }
    }
}

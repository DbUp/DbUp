using System;
using System.IO;
using System.Text;
using DbUp.Engine;

namespace DbUp.Helpers
{
    public static class UpgradeEngineHtmlReport
    {
        /// <summary>
        /// This method will generate an HTML report which can be uploaded as an artifact to any deployment / build tool which supports artifacts.  Useful for getting approvals and seeing a history of what ran when
        /// </summary>
        /// <param name="upgradeEngine">The upgrade engine</param>
        /// <param name="fullPath">The full path of the file which will be generated</param>        
        public static void GenerateUpgradeHtmlReport(this UpgradeEngine upgradeEngine, string fullPath)
        {
            GenerateUpgradeHtmlReport(upgradeEngine, fullPath, string.Empty, string.Empty);
        }

        /// <summary>
        /// This method will generate an HTML report which can be uploaded as an artifact to any deployment / build tool which supports artifacts.  Useful for getting approvals and seeing a history of what ran when
        /// </summary>
        /// <param name="upgradeEngine">The upgrade engine</param>
        /// <param name="fullPath">The full path of the file which will be generated</param>
        /// <param name="serverName">The name of the server being connected to</param>
        /// <param name="databaseName">The name of the database being upgraded</param>
        public static void GenerateUpgradeHtmlReport(this UpgradeEngine upgradeEngine, string fullPath, string serverName, string databaseName)
        {
            var scriptsToRunList = upgradeEngine.GetScriptsToExecute();
            var htmlReport = new StringBuilder();

            htmlReport.Append(GetHtmlHeader(serverName, databaseName));

            for (var i = 0; i < scriptsToRunList.Count; i++)
            {
                htmlReport.Append(GetHtmlForScript(scriptsToRunList[i], i));
            }

            htmlReport.Append(GetHtmlFooter());

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            File.WriteAllText(fullPath, htmlReport.ToString(), DbUpDefaults.DefaultEncoding);
        }

        static string GetHtmlHeader(string serverName, string databaseName)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">                  
    <meta http-equiv=""Content-Language"" content=""en"">
    <link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css"" integrity=""sha384-MCw98/SFnGE8fJT3GXwEOngsV7Zt27NXFoaoApmYm81iuXoPkFOJwJ8ERdknLPMO"" crossorigin=""anonymous"">    
    <title></title>               
    <style> 
    </style>
</head>
<body>
    <noscript>
            <div class=""alert alert-danger"">& nbsp;JavaScript execution is currently disabled. Enable JavaScript on your browser to view the change report.</div>
    </noscript>
	<nav class=""navbar navbar-expand-lg navbar-light bg-light"">
		<a class=""navbar-brand"" href=""#"">DBUp Delta Report Generated {DateTime.Now.ToString()}{(string.IsNullOrEmpty(serverName) == false ? " to upgrade " + serverName + "." + databaseName : string.Empty)}</a>
	</nav>
    <div class=""jumbotron"">
        <h2>DBUp Delta Report</h2>
        <p class=""lead"">The below scripts will run in the order listed below{(string.IsNullOrEmpty(serverName) == false ? " to upgrade " + serverName + "." + databaseName : string.Empty)}</p>
        <hr />     
        <a href=""#"" class=""expandAll"">Expand all</a> |  <a href=""#"" class=""collapseAll"">Collapse all</a>
    </div>

    <div class=""accordion"" id=""accordion"">
";
        }

        static string GetHtmlForScript(SqlScript sqlScript, int counter)
        {
            return $@"<div class=""card"">
			<div class=""card-header"" id=""script{counter}"">
				<h5>
					<button class=""btn btn-link"" type=""button"" data-toggle=""collapse"" data-target=""#script-contents{counter}"">
						{sqlScript.Name}
					</button>
				</h5>
			</div>
			
		    <div id=""script-contents{counter}"" class=""collapse"">
			  <div class=""card-body"">
				<pre class=""prettyprint"">
					<code class=""lang-sql"">
{sqlScript.Contents}
					</code>
				</pre>
			  </div>
			</div>
		</div>";
        }

        static string GetHtmlFooter()
        {
            return @"
    </div>
    <script src=""https://code.jquery.com/jquery-3.3.1.slim.min.js"" integrity=""sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo"" crossorigin=""anonymous""></script>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js"" integrity=""sha384-ZMP7rVo3mIykV+2+9J3UJ46jBk0WLaUAdn689aCwoqbBJiSnjAK/l8WvCWPIPm49"" crossorigin=""anonymous""></script>
    <script src=""https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/js/bootstrap.min.js"" integrity=""sha384-ChfqqxuZUCnJSK3+MXmPNIyE6ZbWh2IMqE241rYiqJxyMiZ6OW/JmZQ5stwEULTy"" crossorigin=""anonymous""></script>   
    <script src=""https://cdn.rawgit.com/google/code-prettify/master/loader/run_prettify.js""></script> 
    <script src=""https://cdn.rawgit.com/google/code-prettify/master/src/lang-sql.js""></script>
    <script>
        $(document).ready(function() {
		     $('.collapseAll').click(function(){
				$('.collapse.show').each(function(){
					$(this).collapse('hide');
				});
			});
			$('.expandAll').click(function(){
				$('.collapse:not("".show"")').each(function(){            
                $(this).collapse('show');
        });
    });
			 
});
    </script>
  </body>
</html>";
        }
    }
}

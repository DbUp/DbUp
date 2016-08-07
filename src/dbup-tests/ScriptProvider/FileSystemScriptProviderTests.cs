#if !NETCORE
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.ScriptProviders;
using DbUp.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;

namespace DbUp.Tests.ScriptProvider
{
    public class FileSystemScriptProviderTests
    {
        public class when_returning_scripts_from_a_directory : SpecificationFor<FileSystemScriptProvider>, IDisposable
        {
            private string testPath;
            private IEnumerable<SqlScript> filesToExecute;

            public override FileSystemScriptProvider Given()
            {
                CreateTestFiles();

                return new FileSystemScriptProvider(testPath);
            }

            private void CreateTestFiles()
            {
                var assembly = Assembly.GetExecutingAssembly();
                var directory = new FileInfo(assembly.Location).DirectoryName;

                testPath = Path.Combine(directory, "sqlfiles");
                Directory.CreateDirectory(testPath);

                foreach (var scriptName in assembly.GetManifestResourceNames().Where(f => f.Contains(".sql")))
                {
                    using (var stream = assembly.GetManifestResourceStream(scriptName))
                    {
                        var filePath = Path.Combine(testPath, scriptName);
                        using (var writer = new FileStream(filePath, FileMode.CreateNew))
                        {
                            stream.CopyTo(writer);
                            writer.Flush();
                            writer.Close();
                        }
                        stream.Close();
                    }
                }
            }

            public override void When()
            {
                filesToExecute = Subject.GetScripts(Substitute.For<IConnectionManager>());
            }

            [Then]
            public void it_should_return_all_sql_files()
            {
                filesToExecute.Count().ShouldBe(5);
            }

            [Then]
            public void the_file_should_contain_content()
            {
                filesToExecute.ShouldAllBe(s => s.Contents.Length > 0);
            }

            [Then]
            public void the_files_should_be_correctly_ordered()
            {
                filesToExecute.First().Name.ShouldEndWith("20110301_1_Test1.sql");
                filesToExecute.Last().Name.ShouldEndWith("Script20130525_2_Test5.sql");
            }

            [Then]
            public void encoding_reader_is_correct()
            {
                // UTF8 encoding
                filesToExecute.Single(f => f.Name.EndsWith("Script20130525_2_Test5.sql"))
                    .Contents
                    .ShouldBe("é");
            }

            public void Dispose()
            {
                Directory.Delete(testPath, true);
            }
        }

        public class when_returning_scripts_from_a_directory_and_using_a_filter : SpecificationFor<FileSystemScriptProvider>,
             IDisposable
        {
            string testPath;
            Func<string, bool> filter;
            IEnumerable<SqlScript> filesToExecute;
            bool filterExecuted;

            public override FileSystemScriptProvider Given()
            {
                CreateTestFiles();
                // Given a filter is provided..
                filter = (a) =>
                {
                    filterExecuted = true;
                    return true;
                };
                return new FileSystemScriptProvider(testPath, filter);
            }

            private void CreateTestFiles()
            {
                var assembly = Assembly.GetExecutingAssembly();
                var directory = new FileInfo(assembly.Location).DirectoryName;

                testPath = Path.Combine(directory, "sqlfiles");
                Directory.CreateDirectory(testPath);


                foreach (var scriptName in assembly.GetManifestResourceNames().Where(f => f.Contains(".sql")))
                {
                    using (var stream = assembly.GetManifestResourceStream(scriptName))
                    {
                        var filePath = Path.Combine(testPath, scriptName);
                        using (var writer = new FileStream(filePath, FileMode.CreateNew))
                        {

                            stream.CopyTo(writer);
                            writer.Flush();
                            writer.Close();
                        }
                        stream.Close();
                    }
                }
            }

            public override void When()
            {
                filesToExecute = Subject.GetScripts(Substitute.For<IConnectionManager>());
            }

            [Then]
            public void the_filter_should_have_been_executed()
            {
                filterExecuted.ShouldBe(true);
            }

            [Then]
            public void the_filter_should_not_interfere_with_script_order()
            {
                filesToExecute.First().Name.ShouldEndWith("20110301_1_Test1.sql");
                filesToExecute.Last().Name.ShouldEndWith("Script20130525_2_Test5.sql");
            }

            public void Dispose()
            {
                Directory.Delete(testPath, true);
            }
        }
    }
}
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.ScriptProviders;
using NSubstitute;
using NUnit.Framework;

namespace DbUp.Tests
{
    public class FileSystemScriptProviderTests
    {
        [TestFixture]
        public class when_returning_scripts_from_a_directory : SpecificationFor<FileSystemScriptProvider>
        {
            private string testPath;
            private IEnumerable<SqlScript> filesToExecute;

            public override FileSystemScriptProvider Given()
            {
                CreateTestFiles();


                return new FileSystemScriptProvider(testPath);
            }

            [TearDown]
            public void CleanUp()
            {
                Directory.Delete(testPath, true);
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
                filesToExecute = Subject.GetScripts(Arg.Any<IConnectionManager>());
            }

            [Then]
            public void it_should_return_all_sql_files()
            {
                Assert.AreEqual(3, filesToExecute.Count());
            }

            [Then]
            public void the_file_should_contain_content()
            {
                foreach (var sqlScript in filesToExecute)
                {
                    Assert.IsTrue(sqlScript.Contents.Length > 0);
                }
            }

            [Then]
            public void the_files_should_be_correctly_ordered()
            {
                Assert.That(filesToExecute.First().Name.EndsWith("20110301_1_Test1.sql"));
                Assert.That(filesToExecute.Last().Name.EndsWith("20110302_1_Test3.sql"));
            }
        }
    }
}
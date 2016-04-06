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
using NUnit.Framework;

namespace DbUp.Tests.ScriptProvider
{
    public class VersionFoldersScriptProviderTests
    {
        [TestFixture]
        public class when_returning_scripts_from_version_folders : SpecificationFor<VersionFoldersScriptProvider>
        {
            private string testPath;
            private IEnumerable<SqlScript> filesToExecute;

            public override VersionFoldersScriptProvider Given()
            {
                CreateTestFiles();


                return new VersionFoldersScriptProvider(testPath);
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

                var subpath1 = Path.Combine(testPath, "20150101-apple");
                Directory.CreateDirectory(subpath1);

                var subpath2 = Path.Combine(testPath, "20150110-banana");
                Directory.CreateDirectory(subpath2);

                var index = 0;
                foreach (var scriptName in assembly.GetManifestResourceNames().Where(f => f.Contains(".sql")))
                {
                    using (var stream = assembly.GetManifestResourceStream(scriptName))
                    {
                        // split files in two separate version directories
                        var destPath = index < 1 ? subpath1 : subpath2;

                        var filePath = Path.Combine(destPath, scriptName);
                        using (var writer = new FileStream(filePath, FileMode.CreateNew))
                        {

                            stream.CopyTo(writer);
                            writer.Flush();
                            writer.Close();
                        }
                        stream.Close();
                    }

                    index++;
                }
            }

            public override void When()
            {
                filesToExecute = Subject.GetScripts(Arg.Any<IConnectionManager>());
            }

            [Then]
            public void it_should_return_all_sql_files()
            {
                Assert.AreEqual(5, filesToExecute.Count());
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
            public void the_files_should_have_correct_folder_prefix()
            {
                Assert.That(filesToExecute.ElementAt(0).Name.EndsWith(@"20150101-apple\DbUp.Tests.TestScripts.Script20110301_1_Test1.sql"));
                Assert.That(filesToExecute.ElementAt(1).Name.EndsWith(@"20150110-banana\DbUp.Tests.TestScripts.Script20110301_2_Test2.sql"));
                Assert.That(filesToExecute.ElementAt(2).Name.EndsWith(@"20150110-banana\DbUp.Tests.TestScripts.Script20110302_1_Test3.sql"));
                Assert.That(filesToExecute.ElementAt(3).Name.EndsWith(@"20150110-banana\DbUp.Tests.TestScripts.Script20130525_1_Test5.sql"));
                Assert.That(filesToExecute.ElementAt(4).Name.EndsWith(@"20150110-banana\DbUp.Tests.TestScripts.Script20130525_2_Test5.sql"));
            }

            [Then]
            public void the_files_should_be_correctly_ordered()
            {
                Assert.That(filesToExecute.First().Name.EndsWith(@"20150101-apple\DbUp.Tests.TestScripts.Script20110301_1_Test1.sql"));
                Assert.That(filesToExecute.Last().Name.EndsWith(@"20150110-banana\DbUp.Tests.TestScripts.Script20130525_2_Test5.sql"));
            }

            [Then]
            public void encoding_reader_is_correct()
            {
                // ANSI encoding
                Assert.AreEqual("é", filesToExecute.Single(f => f.Name.EndsWith("Script20130525_1_Test5.sql")).Contents);

                // UTF8 encoding
                Assert.AreEqual("é", filesToExecute.Single(f => f.Name.EndsWith("Script20130525_2_Test5.sql")).Contents);
            }
        }

        [TestFixture]
        public class when_returning_scripts_from_version_folders_and_using_a_target_version : SpecificationFor<VersionFoldersScriptProvider>
        {
            private string testPath;
            private Func<string, bool> filter = null;
            private string targetVersion;
            private IEnumerable<SqlScript> filesToExecute;

            public override VersionFoldersScriptProvider Given()
            {
                CreateTestFiles();

                targetVersion = "20150101-apple";

                return new VersionFoldersScriptProvider(testPath, filter, targetVersion);
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

                var subpath1 = Path.Combine(testPath, "20150101-apple");
                Directory.CreateDirectory(subpath1);

                var subpath2 = Path.Combine(testPath, "20150110-banana");
                Directory.CreateDirectory(subpath2);

                var index = 0;
                foreach (var scriptName in assembly.GetManifestResourceNames().Where(f => f.Contains(".sql")))
                {
                    using (var stream = assembly.GetManifestResourceStream(scriptName))
                    {
                        // split files in two separate version directories
                        var destPath = index < 1 ? subpath1 : subpath2;

                        var filePath = Path.Combine(destPath, scriptName);
                        using (var writer = new FileStream(filePath, FileMode.CreateNew))
                        {

                            stream.CopyTo(writer);
                            writer.Flush();
                            writer.Close();
                        }
                        stream.Close();
                    }

                    index++;
                }
            }

            public override void When()
            {
                filesToExecute = Subject.GetScripts(Arg.Any<IConnectionManager>());
            }

            [Then]
            public void the_target_version_should_have_been_applied()
            {
                Assert.That(filesToExecute.Count() == 1);
                Assert.That(filesToExecute.ElementAt(0).Name.EndsWith(@"20150101-apple\DbUp.Tests.TestScripts.Script20110301_1_Test1.sql"));
            }
        }

        [TestFixture]
        public class when_returning_scripts_from_version_folders_and_using_a_target_version2 : SpecificationFor<VersionFoldersScriptProvider>
        {
            private string testPath = null;
            private Func<string, bool> filter = null;
            private string targetVersion;
            private IEnumerable<SqlScript> filesToExecute;

            public override VersionFoldersScriptProvider Given()
            {
                CreateTestFiles();

                targetVersion = "3.6.0.1";

                return new VersionFoldersScriptProvider(testPath, filter, targetVersion);
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

                var subpath1 = Path.Combine(testPath, "3 4");
                Directory.CreateDirectory(subpath1);

                var subpath2 = Path.Combine(testPath, "3-5_0~2");
                Directory.CreateDirectory(subpath2);

                var subpath3 = Path.Combine(testPath, "3,6.0");
                Directory.CreateDirectory(subpath3);

                var subpath4 = Path.Combine(testPath, "3.6.0.1");
                Directory.CreateDirectory(subpath4);

                var subpath5 = Path.Combine(testPath, "3.7");
                Directory.CreateDirectory(subpath5);

                var paths = new List<string>() { subpath1, subpath2, subpath3, subpath4, subpath5 };

                var index = 0;
                foreach (var scriptName in assembly.GetManifestResourceNames().Where(f => f.Contains(".sql")).OrderBy(f => f))
                {
                    using (var stream = assembly.GetManifestResourceStream(scriptName))
                    {
                        // split files in two separate version directories
                        var destPath = paths[ index ];

                        var filePath = Path.Combine(destPath, scriptName);
                        using (var writer = new FileStream(filePath, FileMode.CreateNew))
                        {

                            stream.CopyTo(writer);
                            writer.Flush();
                            writer.Close();
                        }
                        stream.Close();
                    }

                    index++;
                }
            }

            public override void When()
            {
                filesToExecute = Subject.GetScripts(Arg.Any<IConnectionManager>());
            }

            [Then]
            public void the_target_version_should_have_been_applied_precisely()
            {
                Assert.That(filesToExecute.Count() == 4);
                Assert.That(filesToExecute.ElementAt(0).Name.EndsWith(@"3 4\DbUp.Tests.TestScripts.Script20110301_1_Test1.sql"));
                Assert.That(filesToExecute.ElementAt(1).Name.EndsWith(@"3,6.0\DbUp.Tests.TestScripts.Script20110302_1_Test3.sql"));
                Assert.That(filesToExecute.ElementAt(2).Name.EndsWith(@"3-5_0~2\DbUp.Tests.TestScripts.Script20110301_2_Test2.sql"));
                Assert.That(filesToExecute.ElementAt(3).Name.EndsWith(@"3.6.0.1\DbUp.Tests.TestScripts.Script20130525_1_Test5.sql"));
            }
        }

        [TestFixture]
        public class when_returning_scripts_from_version_folders_and_using_a_filter_and_target_version : SpecificationFor<VersionFoldersScriptProvider>
        {
            private string testPath;
            private bool _FilterExecuted = false;
            private Func<string, bool> filter;

            private string targetVersion;
            private IEnumerable<SqlScript> filesToExecute;

            public override VersionFoldersScriptProvider Given()
            {
                CreateTestFiles();

                // Given a filter is provided..
                filter = (a) =>
                {
                    _FilterExecuted = true;
                    return
                        !a.EndsWith(@"20150101-apple\DbUp.Tests.TestScripts.Script20110301_2_Test2.sql") &&
                        !a.Equals("folder_containing_invalid_scripts");
                };

                targetVersion = "20150101-apple";

                return new VersionFoldersScriptProvider(testPath, filter, targetVersion);
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

                var subpath1 = Path.Combine(testPath, "20150101-apple");
                Directory.CreateDirectory(subpath1);

                var subpath2 = Path.Combine(testPath, "20150110-banana");
                Directory.CreateDirectory(subpath2);

                var subpath3 = Path.Combine(testPath, "folder_containing_invalid_scripts");
                Directory.CreateDirectory(subpath3);

                var index = 0;
                foreach (var scriptName in assembly.GetManifestResourceNames().Where(f => f.Contains(".sql")))
                {
                    using (var stream = assembly.GetManifestResourceStream(scriptName))
                    {
                        // split files in three separate version directories
                        var destPath = index < 3 ?
                            subpath1 :
                            index > 3 ?
                                subpath3 :
                                subpath2;

                        var filePath = Path.Combine(destPath, scriptName);
                        using (var writer = new FileStream(filePath, FileMode.CreateNew))
                        {

                            stream.CopyTo(writer);
                            writer.Flush();
                            writer.Close();
                        }
                        stream.Close();
                    }

                    index++;
                }
            }

            public override void When()
            {
                filesToExecute = Subject.GetScripts(Arg.Any<IConnectionManager>());
            }

            [Then]
            public void the_filter_should_have_been_executed()
            {
                Assert.IsTrue(_FilterExecuted);
            }

            [Then]
            public void the_target_version_and_filter_should_have_been_applied_and_order_is_correct()
            {
                Assert.That(filesToExecute.Count() == 2);
                Assert.That(filesToExecute.ElementAt(0).Name.EndsWith(@"20150101-apple\DbUp.Tests.TestScripts.Script20110301_1_Test1.sql"));
                Assert.That(filesToExecute.ElementAt(1).Name.EndsWith(@"20150101-apple\DbUp.Tests.TestScripts.Script20110302_1_Test3.sql"));
            }
        }

        [TestFixture]
        public class when_returning_scripts_from_version_folders_and_using_an_invalid_target_version : SpecificationFor<VersionFoldersScriptProvider>
        {
            private string testPath = null;
            private Func<string, bool> filter = null;
            private string targetVersion;
            private IEnumerable<SqlScript> filesToExecute;
            private bool invalidOperationExceptionThrown = false;

            public override VersionFoldersScriptProvider Given()
            {
                CreateTestFiles();

                targetVersion = "INVALID";

                return new VersionFoldersScriptProvider(testPath, filter, targetVersion);
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

                var subpath1 = Path.Combine(testPath, "3.4");
                Directory.CreateDirectory(subpath1);

                foreach (var scriptName in assembly.GetManifestResourceNames().Where(f => f.Contains(".sql")))
                {
                    using (var stream = assembly.GetManifestResourceStream(scriptName))
                    {
                        // split files in two separate version directories
                        var filePath = Path.Combine(subpath1, scriptName);
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
                try
                {
                    filesToExecute = Subject.GetScripts(Arg.Any<IConnectionManager>());
                }
                catch (InvalidOperationException)
                {
                    invalidOperationExceptionThrown = true;
                }
            }

            [Then]
            public void should_throw_on_invalid_target_version()
            {
                Assert.That(invalidOperationExceptionThrown);
            }
        }

        [TestFixture]
        public class when_returning_scripts_from_ambiguous_folder_versions_and_using_a_target_version : SpecificationFor<VersionFoldersScriptProvider>
        {
            private string testPath = null;
            private Func<string, bool> filter = null;
            private string targetVersion = "20150101_1-apple";
            private IEnumerable<SqlScript> filesToExecute;
            private bool invalidOperationExceptionThrown = false;

            public override VersionFoldersScriptProvider Given()
            {
                CreateTestFiles();

                return new VersionFoldersScriptProvider(testPath, filter, targetVersion);
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

                var subpath1 = Path.Combine(testPath, "20150101_1-apple");
                Directory.CreateDirectory(subpath1);

                var subpath2 = Path.Combine(testPath, "20150101-1-banana");
                Directory.CreateDirectory(subpath2);

                var index = 0;
                foreach (var scriptName in assembly.GetManifestResourceNames().Where(f => f.Contains(".sql")))
                {
                    using (var stream = assembly.GetManifestResourceStream(scriptName))
                    {
                        // split files in two separate version directories
                        var destPath = index < 1 ? subpath1 : subpath2;

                        var filePath = Path.Combine(destPath, scriptName);
                        using (var writer = new FileStream(filePath, FileMode.CreateNew))
                        {

                            stream.CopyTo(writer);
                            writer.Flush();
                            writer.Close();
                        }
                        stream.Close();
                    }

                    index++;
                }
            }

            public override void When()
            {
                try
                {
                    filesToExecute = Subject.GetScripts(Arg.Any<IConnectionManager>());
                }
                catch (InvalidOperationException)
                {
                    invalidOperationExceptionThrown = true;
                }
            }

            [Then]
            public void should_throw_on_ambiguous_version_folders()
            {
                Assert.That(invalidOperationExceptionThrown);
            }
        }
    }
}
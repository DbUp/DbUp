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
using Shouldly;

namespace DbUp.Tests.ScriptProvider
{
    public class FileSystemScriptProviderTests
    {
        [TestFixture]
        public class when_options_are_invalid
        {
            [Test]
            public void it_should_throw_when_empty_options()
            {
                Should.Throw<ArgumentNullException>(() => { new FileSystemScriptProvider("Whatever", (FileSystemScriptOptions) null); });
            }
        }

        [TestFixture]
        public class when_returning_scripts_from_a_directory : SpecificationFor<FileSystemScriptProvider>
        {
            private string testPath;
            private IEnumerable<SqlScript> filesToExecute;

            public override FileSystemScriptProvider Given()
            {
                TestScripts.Create(out testPath);
                return new FileSystemScriptProvider(testPath);
            }

            [TearDown]
            public void CleanUp()
            {
                Directory.Delete(testPath, true);
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
            public void the_files_should_be_correctly_ordered()
            {
                Assert.That(filesToExecute.First().Name.EndsWith("20110301_1_Test1.sql"));
                Assert.That(filesToExecute.Last().Name.EndsWith("Script20130525_2_Test5.sql"));
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
        public class when_returning_scripts_from_a_directory_and_using_a_filter : SpecificationFor<FileSystemScriptProvider>
        {
            private string testPath;
            private IEnumerable<SqlScript> filesToExecute;
            private bool _FilterExecuted = false;
            private FileSystemScriptOptions options;

            public override FileSystemScriptProvider Given()
            {
                TestScripts.Create(out testPath);
                // Given a filter is provided..
                options = new FileSystemScriptOptions() {
                    Filter = (a) =>
                    {
                        _FilterExecuted = true;
                        return true;
                    }
                };
                return new FileSystemScriptProvider(testPath, options);
            }

            [TearDown]
            public void CleanUp()
            {
                Directory.Delete(testPath, true);
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
            public void the_filter_should_not_interfere_with_script_order()
            {
                Assert.That(filesToExecute.First().Name.EndsWith("20110301_1_Test1.sql"));
                Assert.That(filesToExecute.Last().Name.EndsWith("Script20130525_2_Test5.sql"));
            }
          
        }

        [TestFixture]
        public class when_returning_scripts_from_a_directory_and_using_subdirectories_option : SpecificationFor<FileSystemScriptProvider>
        {
            private string testPath;
            private IEnumerable<SqlScript> filesToExecute;

            public override FileSystemScriptProvider Given()
            {
                TestScripts.Create(out testPath);
                var options = new FileSystemScriptOptions() {IncludeSubDirectories = true};
                return new FileSystemScriptProvider(testPath, options);
            }

            [TearDown]
            public void CleanUp()
            {
                Directory.Delete(testPath, true);
            }

            public override void When()
            {
                filesToExecute = Subject.GetScripts(Arg.Any<IConnectionManager>());
            }


            [Then]
            public void it_should_return_all_sql_files()
            {
                Assert.AreEqual(9, filesToExecute.Count());
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
            public void the_files_should_be_correctly_ordered_with_subdirectory_order()
            {
                filesToExecute.ElementAt(0).Name.ShouldEndWith("Script20110301_1_Test1.sql");
                filesToExecute.ElementAt(1).Name.ShouldEndWith("Script20110301_2_Test2.sql");
                filesToExecute.ElementAt(2).Name.ShouldEndWith("Script20110302_1_Test3.sql");
                filesToExecute.ElementAt(3).Name.ShouldEndWith("Script20130525_1_Test5.sql");
                filesToExecute.ElementAt(4).Name.ShouldEndWith("Script20130525_2_Test5.sql");
                filesToExecute.ElementAt(5).Name.ShouldEndWith("Test1__9.sql");
                filesToExecute.ElementAt(6).Name.ShouldEndWith("Test2__9.sql");
                filesToExecute.ElementAt(7).Name.ShouldEndWith("Test1__1.sql");
                filesToExecute.ElementAt(8).Name.ShouldEndWith("Test2__1.sql");
            }

        }
    }
}
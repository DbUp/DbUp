using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.ScriptProviders;
using DbUp.Tests.TestInfrastructure;
using NSubstitute;
using Shouldly;
using Xunit;

namespace DbUp.Tests.ScriptProvider
{
    public class FileSystemScriptProviderTests
    {
        public class when_options_are_invalid
        {
            [Fact]
            public void it_should_throw_when_empty_options()
            {
                Should.Throw<ArgumentNullException>(() => { new FileSystemScriptProvider("Whatever", null); });
            }
        }

        public class when_returning_scripts_from_a_directory : SpecificationFor<FileSystemScriptProvider>, IDisposable
        {
            string testPath;
            IEnumerable<SqlScript> filesToExecute;

            public override FileSystemScriptProvider Given()
            {
                TestScripts.Create(out testPath);

                return new FileSystemScriptProvider(testPath);
            }

            protected override void When()
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
            IEnumerable<SqlScript> filesToExecute;
            bool filterExecuted;
            FileSystemScriptOptions options;

            public override FileSystemScriptProvider Given()
            {
                TestScripts.Create(out testPath);
                // Given a filter is provided..
                options = new FileSystemScriptOptions()
                {
                    Filter = (a) =>
                {
                    filterExecuted = true;
                    return true;
                }
                };
                return new FileSystemScriptProvider(testPath, options);
            }

            protected override void When()
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

        public class when_returning_scripts_from_a_directory_and_using_subdirectories_option : SpecificationFor<FileSystemScriptProvider>, IDisposable
        {
            string testPath;
            IEnumerable<SqlScript> filesToExecute;

            public override FileSystemScriptProvider Given()
            {
                TestScripts.Create(out testPath);
                var options = new FileSystemScriptOptions() { IncludeSubDirectories = true };
                return new FileSystemScriptProvider(testPath, options);
            }

            protected override void When()
            {
                filesToExecute = Subject.GetScripts(Substitute.For<IConnectionManager>());
            }

            [Then]
            public void it_should_return_all_sql_files()
            {
                filesToExecute.Count().ShouldBe(9);
            }

            [Then]
            public void the_file_should_contain_content()
            {
                foreach (var sqlScript in filesToExecute)
                {
                    sqlScript.Contents.Length.ShouldBeGreaterThan(0);
                }
            }

            [Then]
            public void the_files_should_contain_the_subfolder_name()
            {
                filesToExecute
                        .Select(f => f.Name)
                        .ShouldContain("Folder1.dbup-tests.TestScripts.Test1__9.sql");
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

            public void Dispose()
            {
                Directory.Delete(testPath, true);
            }
        }

        [Fact]
        public void options_should_include_sql()
        {
            var options = new FileSystemScriptOptions();
            options.Extensions.ShouldContain("*.sql");
        }
    }
}
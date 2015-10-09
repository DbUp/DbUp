It is not uncommon for developers to forget to set the Build Action 
to `Embedded Resource`. This error can be mitigated by adding the following
action to the post-build event in your `.csproj` file.

    <Target Name="AfterBuild">
      <Message Text="@(Content)" Importance="high" Condition="%(Content.Extension) == '.sql'" />
      <Error Condition="%(Content.Extension) == '.sql'" 
             Text="Nothing should be marked as Content, check your scripts are marked as Embedded Resource" />
    </Target>


You can have DbUp add this script for you via Package Manager Console

    PM> Add-DbUpEmbeddedCheck -ProjectName SomeSolution.DataMigration

or  alternatively, you could pick the project from the Default project dropdown, and then run

    PM> Add-DbUpEmbeddedCheck

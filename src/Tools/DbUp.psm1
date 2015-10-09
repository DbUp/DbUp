function Add-DbUpEmbeddedCheck
{
    param($project)

    if (-Not $project) 
    {
        $project = Get-Project
    }

    $ProjectName = $project.Name

    # Need to load MSBuild assembly if it's not loaded yet.
    Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

    # Grab the loaded MSBuild project for the project
    $msbuild = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1
 
    Write "Adding target with name DbUpCheck to $ProjectName ..."

    if ($msbuild.Xml.Targets | ? { $_.Name -eq "DbUpCheck" })
    {
        Write "Target with name DbUpCheck already exists in $ProjectName. No action taken."
        return
    }

    # Add a target to fail the build when our targets are not imported
    $target = $msbuild.Xml.AddTarget("DbUpCheck")

    $messageTask = $target.AddTask("Message")
    $messageTask.SetParameter("Text", "@(Content)")
    $messageTask.SetParameter("Importance", "high")
    $messageTask.Condition = "%(Content.Extension) == '.sql'"

    $errorTask = $target.AddTask("Error")
    $errorTask.Condition = "%(Content.Extension) == '.sql'"
    $errorTask.SetParameter("Text", "@(Content) is marked as content. Scripts should be marked as Embedded Resource.")

    $project.Save()

    Write "Added Target with name DbUp check to $ProjectName."

}

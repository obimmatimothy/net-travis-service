#addin "Newtonsoft.Json&version=10.0.3"

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.IO.File;
using System.Linq;
using System.Text;

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");
var verbosity = Argument<string>("verbosity", "Minimal");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var sourceDir = Directory("./src");

var solutions = GetFiles("./**/*.sln");
var projects = new []
{
    new FilePath(sourceDir.Path + "/Functions/TravisService.Functions.HandheldEventProcessor/TravisService.Functions.HandheldEventProcessor.csproj"),
};

var functionNames = new[] { "TravisServiceHandheldEventProcessor"," };

// BUILD OUTPUT DIRECTORIES
var artifactsDir = Directory("./artifacts");
var publishDir = Directory("./publish/");

// VERBOSITY
var dotNetCoreVerbosity = Cake.Common.Tools.DotNetCore.DotNetCoreVerbosity.Normal;
if (!Enum.TryParse(verbosity, true, out dotNetCoreVerbosity))
{
    dotNetCoreVerbosity = Cake.Common.Tools.DotNetCore.DotNetCoreVerbosity.Normal;
    Warning(
        "Verbosity could not be parsed into type 'Cake.Common.Tools.DotNetCore.DotNetCoreVerbosity'. Defaulting to {0}",
        dotNetCoreVerbosity);
}

///////////////////////////////////////////////////////////////////////////////
// Partial scripts
///////////////////////////////////////////////////////////////////////////////
#load "cake/local-tasks.cake";

///////////////////////////////////////////////////////////////////////////////
// COMMON FUNCTION DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

void ExecuteTests(FilePathCollection projectFiles) {
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true
    };
    foreach(var file in projectFiles)
    {
        Information("Testing '{0}'...", file);
        DotNetCoreTest(file.FullPath, settings);
        Information("'{0}' has been tested.", file);
    }
}

FilePathCollection GetUnitTestFiles() {
    var projectFiles = GetFiles("./test/**/*.csproj");
    FilePathCollection unitTestProjects = new FilePathCollection();
    foreach (var file in projectFiles) {
        if (!file.FullPath.Contains("Integration") && !file.FullPath.Contains("Acceptance")) {
            unitTestProjects.Add(file);
        }
    }
    return unitTestProjects;
}

FilePathCollection GetIntegrationTestFiles() {
    var projectFiles = GetFiles("./test/**/*.csproj");
    FilePathCollection integrationTestProjects = new FilePathCollection();
    foreach (var file in projectFiles) {
        if (file.FullPath.Contains("Integration")) {
            integrationTestProjects.Add(file);
        }
    }
    return integrationTestProjects;
}

void EnsureAmazonLambdaToolsInstalled() {
    try {
        DotNetCoreTool("", "tool install", "--tool-path ./tools Amazon.Lambda.Tools");
    } catch{ }
    DotNetCoreTool("", "tool update", "--tool-path ./tools Amazon.Lambda.Tools");
}

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx => {
    // Executed BEFORE the first task.
    EnsureDirectoryExists(artifactsDir);
    EnsureDirectoryExists(publishDir);
    Information("Running tasks...");
});

Teardown(ctx =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Cleans all directories that are used during the build process.")
    .Does(() =>
    {
        foreach(var solution in solutions)
        {
            Information("Cleaning {0}", solution.FullPath);
            CleanDirectories(solution.FullPath + "/**/bin/" + configuration);
            CleanDirectories(solution.FullPath + "/**/obj/" + configuration);
            Information("{0} was cleaned.", solution.FullPath);
        }

        CleanDirectory(artifactsDir);
        CleanDirectory(publishDir);
    });

Task("Restore")
    .Description("Restores all the NuGet packages that are used by the specified solution.")
    .Does(() =>
    {
        var settings = new DotNetCoreRestoreSettings
        {
            DisableParallel = false,
            NoCache = true,
            Verbosity = dotNetCoreVerbosity
        };

        foreach(var solution in solutions)
        {
            Information("Restoring NuGet packages for '{0}'...", solution);
            DotNetCoreRestore(solution.FullPath, settings);
            Information("NuGet packages restored for '{0}'.", solution);
        }
    });

Task("Build")
    .IsDependentOn("Restore")
    .Description("Builds all the different parts of the project.")
    .Does(() =>
    {
        var msBuildSettings = new DotNetCoreMSBuildSettings
        {
            TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error,
            Verbosity = dotNetCoreVerbosity
        };

        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            MSBuildSettings = msBuildSettings,
            NoRestore = true
        };

        foreach(var solution in solutions)
        {
            Information("Building '{0}'...", solution);
            DotNetCoreBuild(solution.FullPath, settings);
            Information("'{0}' has been built.", solution);
        }
    });

Task("Test-All")
    .Description("Tests all the different parts of the project.")
    .IsDependentOn("Test-Unit")
    .IsDependentOn("Test-Integration");

Task("Test-Unit")
    .Description("Runs unit tests.")
    .Does(() =>
    {
        var unitTests = GetUnitTestFiles();
        ExecuteTests(unitTests);
    });

Task("Test-Integration")
    .Description("Runs integration tests.")
    .Does(() =>
    {
        var integrationTests = GetIntegrationTestFiles();
        ExecuteTests(integrationTests);
    });

Task("Publish")
    .IsDependentOn("Build")
    .Description("Build the Lambda Functions.")
    .Does(() =>
    {
        EnsureAmazonLambdaToolsInstalled();
        foreach(var project in projects)
        {
            var projectPath = project.GetDirectory().ToString();
            var projectName = project.GetFilename().ToString().Replace(".csproj", "");

            var packagedOutput = $"{System.IO.Path.Combine(artifactsDir, projectName)}.zip";

            Information("Publishing '{0}'...", projectName);
            var settings = new ProcessSettings
            {
                Arguments = $"package -o {packagedOutput} -pl {projectPath}",
            };
            using(var process = StartAndReturnProcess("./tools/dotnet-lambda", settings))
            {
                process.WaitForExit();
            }
            Information("'{0}' has been published.", projectName);
        }
    });

Task("AwsCliPackage")
    .Description("Calls AWS CLI to Package the Lambda Function.")
    .WithCriteria(!BuildSystem.IsLocalBuild) // don't run it when this is a local build
    .Does(() =>
    {
        var samArtifactsBucket = EnvironmentVariable("SAM_ARTIFACTS_BUCKET");

        if (samArtifactsBucket == null)
        {
            throw new Exception("The SAM_ARTIFACTS_BUCKET environment variable has not been specificed.");
        }

        var settings = new ProcessSettings
        {
            Arguments = $"cloudformation package --template-file cloudformation.yaml --s3-bucket {samArtifactsBucket} --output-template-file packaged.yaml",
        };

        Information("Starting AWS CLI Packaging of Lambda Function");
        using(var process = StartAndReturnProcess("aws", settings))
        {
            process.WaitForExit();
            Information("Exit code: {0}", process.GetExitCode());
        }
        Information("AWS CLI Package has finished.");
    });

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("buildrelease")
    .Description("This is the task which will run if target Package is passed in.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test-Unit")
    .IsDependentOn("Publish")
    .IsDependentOn("AwsCliPackage")
    .Does(() => { Information("Package target ran."); });

Task("pullrequest")
    .Description("This is the task which will run if target Test is passed in.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test-All")
    .Does(() => { Information("Test target ran."); });

Task("Default")
    .Description("This is the default task which will run if no specific target is passed in.")
    .IsDependentOn("buildrelease");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);

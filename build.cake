#addin "Cake.Docker"
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var builderContainerName = Argument("BuilderContainerName", "");
var testResultsPath = Argument("TestResultsPath", "");

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var packagesDir = ".";
var packageVersion = "1.0.0";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore("Payments.sln");
}); 

Task("Build")
    .Does(() =>
{
    DotNetCoreBuild(
        "Payments.sln",
        new DotNetCoreBuildSettings
        {
            Configuration = configuration
        });
});

Task("RunTestsInDocker")
    .Does(() =>
{
    DockerRun(
        new DockerContainerRunSettings {
            Env = new string [] { $"TEST_RESULTS_PATH={testResultsPath}" },
            VolumesFrom = new string[] { builderContainerName },
            Workdir = "/src"
        },
        "docker/compose:1.24.1",
        "-f docker-compose.testrunner.yml run --rm testrunner"
    );
})
.Finally(() =>
{
    DockerRun(
        new DockerContainerRunSettings {
            VolumesFrom = new string[] { builderContainerName },
            Workdir = "/src"
        },
        "docker/compose:1.24.1",
        "-f docker-compose.testrunner.yml run --rm testrunner"
    );
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
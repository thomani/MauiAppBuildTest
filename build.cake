var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var outputPath = "./publish/android/";

Task("Clean")
    .Does(() =>
{
    CleanDirectory(outputPath);
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    Information("Building an app project without a Maui Class Library works.");
    DotNetBuild("./MauiAppBuildTest/MauiAppBuildTestNoLib.csproj", new DotNetBuildSettings
    {
        Configuration = configuration,
        Framework = "net9.0-android",
        OutputDirectory = outputPath
    });

    Information("Building a Maui Class Library on it's own works.");
    DotNetBuild("./BuildTestLib/BuildTestLib.csproj", new DotNetBuildSettings
    {
        Configuration = configuration,
        Framework = "net9.0-android",
        OutputDirectory = outputPath
    });

    Information("Building an app project with a Maui Class Library works, when starting dotnet build as a process directly.");
    var appProjectPath = "./MauiAppBuildTest/MauiAppBuildTest.csproj";
    StartProcess(
         "dotnet",
         new ProcessSettings
         {
             Arguments = $"build {appProjectPath} --output {outputPath} --framework net9.0-android --configuration {configuration}",
             RedirectStandardOutput = true,
             RedirectStandardError = true
         },
         out var redirectedStandardOutput,
         out var redirectedErrorOutput
     );
    Information("Output: {0}", string.Join("\n", redirectedStandardOutput));
    Error("Errors: {0}", string.Join("\n", redirectedErrorOutput));

    Information("Building an app project with a Maui Class Library fails, when using cake's DotNetBuild.");
    DotNetBuild(appProjectPath, new DotNetBuildSettings
    {
        Configuration = configuration,
        Framework = "net9.0-android",
        OutputDirectory = outputPath
    });
});

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);

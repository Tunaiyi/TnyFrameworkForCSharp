
#addin "Cake.FileSet"
#addin nuget:?package=Microsoft.Extensions.FileSystemGlobbing&version=2.2.0.0

///////////////////////////////////////////////////////////////////////////////
// Classes
///////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var homePath = Argument("homePath", IsRunningOnWindows() ? $"{EnvironmentVariable("HOMEDRIVE")}/{EnvironmentVariable("HOMEPATH")})" : $"{EnvironmentVariable("HOME")}");

// 当前目录
var currentPath = Argument("currentPath", $"{Context.Environment.WorkingDirectory}");

// 方案路径
var projectSln = Argument("projectSln", $"{currentPath}/TnyFramework.sln");


public void BuildProjectSolution(string buildMode) 
{
    NuGetRestore(projectSln, new NuGetRestoreSettings { NoCache = true });
    Information($"Build : {projectSln} By {buildMode}");
    MSBuild($"{projectSln}", new MSBuildSettings {
        Verbosity = Verbosity.Minimal,
        Configuration = buildMode
    });
}

Task("TnyFramework.Build.Publish")
    .Does(() => {
        BuildProjectSolution("Publish");
    });

Task("TnyFramework.Build.Release")
    .Does(() => {
        BuildProjectSolution("Release");
    });
    
RunTarget(target);

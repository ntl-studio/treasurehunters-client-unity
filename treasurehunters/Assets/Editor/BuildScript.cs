using System;
using UnityEditor;

public static class BuildScript 
{
    public static void PerformBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/SampleScene.unity" }
        };
        var args = System.Environment.GetCommandLineArgs();
        var foundBuildPath = false;
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] != "-buildPath") continue;
            buildPlayerOptions.locationPathName = args[i + 1];
            foundBuildPath = true;
        }
        if (!foundBuildPath)
        {
            throw new ArgumentException("Please specify a build path with -buildPath argument");
        }
        buildPlayerOptions.target = BuildTarget.iOS;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}

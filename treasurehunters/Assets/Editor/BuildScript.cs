using UnityEditor;

public static class BuildScript 
{
    public static void PerformBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = "~/projects/ntl-studio/ios";
        buildPlayerOptions.target = BuildTarget.iOS;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}

using UnityEditor;
using UnityEngine;

public static class BuildDiagnostics
{
    [MenuItem("Tools/Run Build Check")]
    public static void TryBuild()
    {
        var report = BuildPipeline.BuildPlayer(
            new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Main.unity" }, // change this
                locationPathName = "BuildCheckTemp",
                target = BuildTarget.StandaloneWindows64,      // change to your target
                options = BuildOptions.Development
            });

        Debug.Log("Build result: " + report.summary.result);
    }
}

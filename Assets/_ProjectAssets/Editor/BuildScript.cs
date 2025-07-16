using System;
using System.IO;
using UnityEditor;

public static class BuildScript
{
    static readonly string[] SCENES = {
        "Assets/_ProjectAssets/Scenes/MainMenu.unity",
        "Assets/_ProjectAssets/Scenes/Survive.unity",
        "Assets/_ProjectAssets/Scenes/LvlsScene.unity",
    };

    public static void PerformBuildAndroid()
    {
        string buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "1";

        // Set version
        PlayerSettings.bundleVersion = $"6.0.{buildNumber}";
        PlayerSettings.Android.bundleVersionCode = int.Parse(buildNumber);

        string buildDirectory = "Builds/Android";
        Directory.CreateDirectory(buildDirectory);
        string pathToBuild = Path.Combine(buildDirectory, $"v1.6.{buildNumber}.apk");

        PlayerSettings.Android.keystoreName = Environment.GetEnvironmentVariable("KEYSTORE_PATH");
        PlayerSettings.Android.keystorePass = Environment.GetEnvironmentVariable("KEYSTORE_PASS");
        PlayerSettings.Android.keyaliasName = Environment.GetEnvironmentVariable("KEY_ALIAS");
        PlayerSettings.Android.keyaliasPass = Environment.GetEnvironmentVariable("KEY_ALIAS_PASS");

        BuildPipeline.BuildPlayer(SCENES, pathToBuild, BuildTarget.Android, BuildOptions.None);
    }
}

using System;
using System.IO;
using UnityEditor;

public static class BuildScript
{
    static readonly string[] SCENES = {
        "_ProjectAssets/Scenes/MainMenu.unity",
        "_ProjectAssets/Scenes/Survive.unity",
        "_ProjectAssets/Scenes/LvlsScene.unity",
    };

    public static void PerformBuildAndroid()
    {
        string buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "local";

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

using System;
using UnityEditor;
using UnityEngine;

public static class BuildScript
{
    static string[] SCENES = { "_ProjectAssets/Scenes/MainMenu.unity",
                               "_ProjectAssets/Scenes/Survive.unity",
                               "_ProjectAssets/Scenes/LvlsScene.unity",};

    public static void PerformBuildAndroid()
    {
        string buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER");
        if (string.IsNullOrEmpty(buildNumber))
        {
            buildNumber = "local";
        }

        string pathToBuild = $"Builds/Android/v1.6.{buildNumber}.apk";

        PlayerSettings.Android.keystoreName = System.Environment.GetEnvironmentVariable("KEYSTORE_PATH");
        PlayerSettings.Android.keystorePass = System.Environment.GetEnvironmentVariable("KEYSTORE_PASS");
        PlayerSettings.Android.keyaliasName = System.Environment.GetEnvironmentVariable("KEY_ALIAS");
        PlayerSettings.Android.keyaliasPass = System.Environment.GetEnvironmentVariable("KEY_ALIAS_PASS");

        BuildPipeline.BuildPlayer(SCENES, pathToBuild, BuildTarget.Android, BuildOptions.None);
    }
}

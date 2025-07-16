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
        string buildNumberStr = Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "1";
        int buildNumber = 1;

        if (!int.TryParse(buildNumberStr, out buildNumber))
        {
            buildNumber = 1;
        }

        int minVersionCode = 618;
        int versionCode = minVersionCode + buildNumber;


        PlayerSettings.bundleVersion = $"1.0.{versionCode}";
        PlayerSettings.Android.bundleVersionCode = versionCode;

        string buildDirectory = "Builds/Android";
        Directory.CreateDirectory(buildDirectory);

        string pathToBuild = Path.Combine(buildDirectory, $"v1.6.{versionCode}.aab");

        PlayerSettings.Android.keystoreName = Environment.GetEnvironmentVariable("KEYSTORE_PATH");
        PlayerSettings.Android.keystorePass = Environment.GetEnvironmentVariable("KEYSTORE_PASS");
        PlayerSettings.Android.keyaliasName = Environment.GetEnvironmentVariable("KEY_ALIAS");
        PlayerSettings.Android.keyaliasPass = Environment.GetEnvironmentVariable("KEY_ALIAS_PASS");

        PlayerSettings.Android.useCustomKeystore = true;
        EditorUserBuildSettings.buildAppBundle = true;

        BuildPipeline.BuildPlayer(SCENES, pathToBuild, BuildTarget.Android, BuildOptions.None);
    }


}

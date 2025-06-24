using System.IO;
using UnityEditor;
using UnityEngine;

public class RefIdFinder : EditorWindow
{
    private string searchRefId = "5610944455733411859";

    [MenuItem("Tools/Find RefId in Assets")]
    public static void ShowWindow()
    {
        GetWindow<RefIdFinder>("Find RefId");
    }

    private void OnGUI()
    {
        GUILayout.Label("Search for RefId in Serialized Assets", EditorStyles.boldLabel);
        searchRefId = EditorGUILayout.TextField("RefId to find", searchRefId);

        if (GUILayout.Button("Search"))
        {
            SearchAssetsForRefId(searchRefId);
        }
    }

    private void SearchAssetsForRefId(string refId)
    {
        string[] assetPaths = Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories);
        int foundCount = 0;

        foreach (string path in assetPaths)
        {
            if (!path.EndsWith(".prefab") &&
                !path.EndsWith(".asset") &&
                !path.EndsWith(".unity") &&
                !path.EndsWith(".controller") &&
                !path.EndsWith(".overrideController"))
            {
                continue;
            }

            string content = File.ReadAllText(path);
            if (content.Contains(refId))
            {
                Debug.Log($"Found RefId in: {path}", AssetDatabase.LoadMainAssetAtPath(path));
                foundCount++;
            }
        }

        if (foundCount == 0)
        {
            Debug.Log($"No assets found containing RefId: {refId}");
        }
    }
}

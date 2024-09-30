using UnityEditor;
using UnityEngine;

public class ListAllSprites : EditorWindow
{
    [MenuItem("Tools/List All Sprites")]
    public static void ShowWindow()
    {
        // 创建一个窗口来显示结果
        GetWindow<ListAllSprites>("List All Sprites");
    }

    private Vector2 scrollPos;

    private void OnGUI()
    {
        if (GUILayout.Button("List Sprites"))
        {
            ListAllSpritesInProject();
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var sprite in sprites)
        {
            EditorGUILayout.ObjectField(sprite, typeof(Sprite), false);
        }
        EditorGUILayout.EndScrollView();
    }

    private static Sprite[] sprites;

    private static void ListAllSpritesInProject()
    {
        // 查找所有类型为 Sprite 的资源
        string[] guids = AssetDatabase.FindAssets("t:Sprite");

        sprites = new Sprite[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }

        Debug.Log($"Found {sprites.Length} sprites in the project.");
    }
}

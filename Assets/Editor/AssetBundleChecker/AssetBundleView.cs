using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using ABChecker;
using System.Linq;

public class AssetBundleView : EditorWindow
{
    private static AssetBundleCheckerData dataBase;
    private static Vector2 mainScroll;
    private GUILayoutOption[] options = new[] { GUILayout.Width(64), GUILayout.Height(64) };
    private int viewWidth = 120;
    private int assetNamespace = 150;
    private int maxLinelength = 7;
    private int assetCellweidth = 200;

    public static void Open()
    {
        dataBase = AssetDatabase.LoadAssetAtPath<AssetBundleCheckerData>("Assets/Editor/AssetBundleChecker/AssetBundleCheckerData.asset");
        GetWindow<AssetBundleView>();
    }

    private void OnGUI()
    {
        if (dataBase == null)
        {
            return;
        }

        if (dataBase.Assets.Count == 0)
        {
            return;
        }

        var lineLength = maxLinelength - dataBase.Assets.Count;
        EditorGUILayout.BeginHorizontal(GUI.skin.window);
        mainScroll = EditorGUILayout.BeginScrollView(mainScroll);
        EditorGUILayout.BeginHorizontal();

        foreach (var assetList in dataBase.Assets)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField(assetList.Key.ToString() + "[" + assetList.Value.Length + "]", GUILayout.Width(viewWidth));
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(assetCellweidth), GUILayout.MaxWidth(assetCellweidth));
            foreach (var asset in assetList.Value.Select((data, index) => new { Data = data, Index = index }))
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.ObjectField(asset.Data, typeof(Sprite), false, options);
                EditorGUILayout.LabelField(asset.Data.name, GUILayout.Width(assetNamespace));
                EditorGUILayout.EndVertical();
                if (asset.Index % lineLength == lineLength-1)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(assetCellweidth), GUILayout.MaxWidth(assetCellweidth));
                }

            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
    }
}
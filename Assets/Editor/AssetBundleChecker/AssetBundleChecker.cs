using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using System.Linq;

namespace ABChecker
{
    public class AssetBundleChecker : EditorWindow
    {

        private static AssetBundleChecker editorWindow; 
        private static AssetBundleCheckerData dataBase;
        private static string preUrl;
        private static string postUrl;

        [MenuItem("Window/AssetBundleCheck/Download Open %a")]
        private static void Open()
        {
            if(editorWindow == null)
            {
                editorWindow = CreateInstance<AssetBundleChecker>();
            }

            editorWindow.ShowUtility();
            dataBase = AssetDatabase.LoadAssetAtPath<AssetBundleCheckerData>("Assets/Editor/AssetBundleChecker/AssetBundleCheckerData.asset");
            preUrl = dataBase.PreUrl;
            postUrl = dataBase.PostUrl;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("AssetBundleのURLを入力してね");
            EditorGUILayout.LabelField(preUrl);
            postUrl = EditorGUILayout.TextField(postUrl);

            if (GUILayout.Button("ダウンロード開始"))
            {
                dataBase.PostUrl = postUrl;
                AssetBundleDownload();
            }

            if (GUILayout.Button("再起動"))
            {
                Open();
            }

            if (GUILayout.Button("データ消去"))
            {
                dataBase.ClearAssetBundle();
            }
        }

        private void AssetBundleDownload()
        {
            var t = Donwload();
            while (t.MoveNext());
        }

        private IEnumerator Donwload()
        {
            var url = preUrl + postUrl;
            if (String.IsNullOrEmpty(postUrl))
            {
                Debug.LogError("このURLは無効です");
                yield break;
            }
            else
            {
                Debug.Log("ダウンロード先:\n" + url);
            }

            using (var request = UnityWebRequest.GetAssetBundle(url))
            {
                yield return request.Send();

                while (!request.isDone)
                {
                    if (request.isError)
                    {
                        Debug.LogError(request.error);
                        yield break;
                    }

                    yield return null;
                }

                if (request.responseCode != 200)
                {
                    if(request.responseCode == 404) 
                    {
                        Debug.LogError("そのURLは無効です");
                    }
                    else
                    {
                        Debug.LogError("エラーです");
                    }

                    yield break;
                }

                var bundle = ((DownloadHandlerAssetBundle)request.downloadHandler).assetBundle;
                if (bundle == null)
                {
                    yield break;
                }

                if(dataBase == null)
                {
                    Debug.LogError("設定ファイルが存在しません");
                    yield break;
                }

                var allAssets = bundle.LoadAllAssets();

                var prefabAssets = allAssets.Where(obj => obj is GameObject)
                    .Cast<GameObject>()
                    .ToArray();
                if (prefabAssets.Any())

                {
                    dataBase.LoadAssetBundle(AssetType.Prefab, prefabAssets);
                }

                var materialAssets = allAssets.Where(obj => obj is Material)
                    .Cast<Material>()
                    .ToArray();
                if (materialAssets.Any())

                {
                    dataBase.LoadAssetBundle(AssetType.Material, materialAssets);
                }

                var spriteAssets = allAssets.Where(obj => obj is Sprite)
                    .Cast<Sprite>()
                    .ToArray();

                if (spriteAssets.Any())
                {
                    dataBase.LoadAssetBundle(AssetType.Sprite, spriteAssets);
                }

                var soundAssets = allAssets.Where(obj => obj is AudioClip)
                    .Cast<AudioClip>()
                    .ToArray();
                if (soundAssets.Any())
                {
                    dataBase.LoadAssetBundle(AssetType.Sound, soundAssets);
                }

                var otherAssets = allAssets.Where(obj => !(obj is GameObject))
                    .Where(obj => !(obj is Material))
                    .Where(obj => !(obj is Sprite))
                    .Where(obj => !(obj is AudioClip))
                    .ToArray();

                if (otherAssets.Any())
                {
                    dataBase.LoadAssetBundle(AssetType.Other, otherAssets);
                }

                AssetBundleView.Open();
                bundle.Unload(false);
            }
        }
    }
}

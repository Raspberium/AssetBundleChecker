using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace ABChecker
{
    public enum AssetType
    {
        Prefab,
        Material,
        Sprite,
        Sound,
        Other
    }

    public class AssetBundleCheckerData : ScriptableObject
    {
        [SerializeField]
        private string preUrl = "https://otogimig.blob.core.windows.net/";
        public string PreUrl { get{ return preUrl; } set { preUrl = value; } }

        [SerializeField]
        private string postUrl = "";
        public string PostUrl { get{ return postUrl; } set{ postUrl = value ; } }

        public Dictionary<AssetType, Object[]> Assets = new Dictionary<AssetType, Object[]>();

        public void LoadAssetBundle(AssetType type, Object[] objects)
        {
            Assets[type] = objects.OrderBy(obj =>
            {
                int parsedInt;
                int.TryParse(obj.name, out parsedInt);
                return parsedInt;
            }).ToArray();
        }

        public void ClearAssetBundle()
        {
            Assets.Clear();
        }
    }
}
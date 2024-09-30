using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace liaoyanxuan.common.interfaces
{
    public interface IAssetLoader
    {
        UnityEngine.Object LoadAssetSync(string assetBundleName, string assetName, System.Type type);

        Sprite LoadUISprite(string assetBundleName, string assetName);

        GameObject LoadUI(string assetBundleName, string assetName, System.Type type);


        UnityEngine.Object loadUIObject(string assetBundleName, string assetName, System.Type type);


        void UnloadUnusedAssets();

    }
}

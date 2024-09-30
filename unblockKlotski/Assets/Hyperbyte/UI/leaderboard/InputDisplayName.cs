using CommonLib.zlib;
using Hyperbyte;
using sw.game.wordfilter;
using sw.util;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class InputDisplayName : MonoBehaviour
{
    // Start is called before the first frame update

    private InputField inputField;

    [SerializeField]
    private ConfirmDisplayName confirmDisplayName;


     void Awake()
    {
        inputField = GetComponentInChildren<InputField>();
        loadEsswordConfig();
    }

    /// <summary>
    /// 屏蔽词库
    /// </summary>
    void loadEsswordConfig()
    {

        string esswordpath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "language" + Path.DirectorySeparatorChar + "essword.dat";

#if UNITY_ANDROID && !UNITY_EDITOR
        esswordpath = "jar://language" + Path.DirectorySeparatorChar + "essword.dat";

#endif

        if (!string.IsNullOrEmpty(esswordpath))
        {
            byte[] bytes = null;
            if (esswordpath.StartsWith("jar://"))
            {
                bytes = AndroidUtil.ReadAssetFile(esswordpath.Substring(6));
            }
            else
            {
                if (File.Exists(esswordpath))
                {
                    bytes = File.ReadAllBytes(esswordpath);
                }

            }

            if (bytes != null)
            {
                byte[] bytes1 = ZlibUtil.Decompress(bytes);
                ByteArray assetBytes = new ByteArray(bytes1);
                WordFilter.Instance.loadData(assetBytes);
            }

        }

    }


    public void InputDisplayNameOK()
    {
        if (InputManager.Instance.canInput())
        {
            UIFeedback.Instance.PlayButtonPressEffect();
            string nameInput = inputField.text.Trim();


            nameInput = WordFilter.Instance.replace(nameInput);

            if (false == string.IsNullOrEmpty(nameInput))
            {
                if (nameInput.IndexOf("*") >= 0)
                {
                    UIController.Instance.ShowAlertTip("昵称中存在敏感词:" + nameInput + ",请重新输入");
                }
                else
                {
                    this.gameObject.SetActive(false);
                    confirmDisplayName.gameObject.SetActive(true);
                    confirmDisplayName.setInputName(nameInput);
                } 
            }
            else
            {
                Toast.instance.ShowMessage("请输入游戏昵称");
            }
 
        }
    }
}

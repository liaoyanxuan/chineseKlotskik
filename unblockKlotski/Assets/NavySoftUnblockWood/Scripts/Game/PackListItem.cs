using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScrollRectFrameWork;
using UnityEngine.UI;
using ScreenFrameWork;
using TMPro;
using Hyperbyte.Localization;
using System;

public class PackListItem : ExpandableListItem<GameModeInfor>
{

    [SerializeField]
    private Image banner;
    [SerializeField]
    private GameObject locked;
    [SerializeField]
    private GameObject showLevel;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI starRequestText;
    [SerializeField]
    private TextMeshProUGUI gameModeText;

    private GameModeInfor dataObject;
    private RectTransform rectLocked;
    private bool unlock = false;

    public override void Initialize(GameModeInfor dataObject)
    {
        rectLocked = locked.GetComponent<RectTransform>();
    }

    public override void Setup(GameModeInfor dataObject, bool isExpanded)
    {
        rectLocked.transform.localScale = Vector3.one;
        banner.sprite = dataObject.bannerGameMode;
        string gameModeTextStr = LocalizationManager.Instance.GetTextWithTag(dataObject.nameGameMode);

        gameModeText.text = gameModeTextStr.Split('_')[0];
        gameModeText.fontSize = Convert.ToInt32(gameModeTextStr.Split('_')[1]);
        unlock = GameManager.instance.GetTotalStarEarnAllMode >= dataObject.starRequest;
        locked.SetActive(!unlock);
        showLevel.SetActive(unlock);
        levelText.text = string.Format("{0}/{1}", (GameManager.instance.GetLevelCompleteMode(dataObject.idGameMode)), dataObject.datas.Count);
        starRequestText.text = dataObject.starRequest.ToString();
        this.dataObject = dataObject;

        if (RatioResolution.GetResolution() <= 1.5f)
        {
            showLevel.GetComponent<RectTransform>().anchoredPosition = new Vector3(15, 0, 0);
            locked.GetComponent<RectTransform>().anchoredPosition = new Vector3(15, 0, 0);
        }
    }

    public override void Removed()
    {

    }

    public override void Collapsed()
    {

    }

    private IEnumerator ScaleBoardLocked()
    {
        float duration = .3f;
     
        UIAnimation scaleX = UIAnimation.ScaleX(rectLocked, 1.2f, duration);
        scaleX.Play();
        UIAnimation scaleY = UIAnimation.ScaleY(rectLocked, 1.2f, duration);
        scaleY.Play();
        yield return new WaitForSeconds(duration);

        scaleX = UIAnimation.ScaleX(rectLocked, 1f, duration);
        scaleX.Play();
        scaleY = UIAnimation.ScaleY(rectLocked, 1f, duration);
        scaleY.Play();
    }

    public void OnEventClicked()
    {
        if (!unlock)
        {
            StartCoroutine(ScaleBoardLocked());
            return;
        }
        GameManager.GAME_MODE = dataObject.nameGameMode;
        GameManager.instance.SelectedGameMode(dataObject.idGameMode);
        ScreenManager.Instance.Show("levels");
    }
}

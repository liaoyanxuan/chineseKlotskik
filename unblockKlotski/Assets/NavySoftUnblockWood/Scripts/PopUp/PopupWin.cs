using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopUpFrameWork;
using TMPro;
using Hyperbyte.Localization;
using UnityEngine.UI;
using Hyperbyte.Ads;

public class PopupWin : Popup
{

    [SerializeField]
    private CanvasGroup buttonsGroup;

    [SerializeField]
    private TextMeshProUGUI moveText;
    [SerializeField]
    private TextMeshProUGUI bestMoveText;

    [SerializeField]
    private Button nextBtn;

    [SerializeField]
    private List<Animation> stars = new List<Animation>();




    public override void OnShowing(object[] inData)
    {
        HideAllStar();
        int star = (int)inData[0];

        buttonsGroup.alpha = 0;
        buttonsGroup.interactable = false;

        StartCoroutine(ShowStar(star));
        moveText.text = string.Format(LocalizationManager.Instance.GetTextWithTag("move:{0}"),(int)inData[1]);
        bestMoveText.text = string.Format(LocalizationManager.Instance.GetTextWithTag("best:{0}"), (int)inData[2]);

        bool isLastGuanka = (bool)inData[3];
        if (isLastGuanka) 
        {
            nextBtn.interactable = false;
        }
        else 
        {
            nextBtn.interactable = true;
        }

        base.OnShowing(inData);
    }

    private IEnumerator ShowStar(int star)
    {
       //GameManager.instance.CurrentLevel.levelStage

        for (int i = 0; i < star; i++)
        {
            yield return new WaitForSeconds(.35f);
         //   stars[i].gameObject.SetActive(true);
            starShow(stars[i].gameObject, true);
            stars[i].Play();
             Timer.Schedule(this, 0.03f * (i+1), () => { SoundManager.instance.StarCompletedSound(); });
            //SoundManager.instance.StarCompletedSound();
        }

        yield return new WaitForSeconds(0.5f);

        buttonsGroup.LeanAlpha(1.0f,0f).setOnComplete(()=> { buttonsGroup.interactable = true; }).setEaseOutQuint();


    }

    private void HideAllStar()
    {
        for (int i = 0; i < stars.Count; i++)
        {
           // stars[i].gameObject.SetActive(false);
            starShow(stars[i].gameObject, false);
            stars[i].transform.localScale = Vector3.zero;
        }

        //int mini = Mathf.CeilToInt(GameManager.instance.CurrentLevel.levelData.blockInfors[0].mini / 10);
        int mini = Mathf.CeilToInt(GameManager.instance.winReplayCurrentLevel.levelData.blockInfors[0].mini / 10f);

       // float totalStar = Mathf.Ceil(limitMove / 10f);

        for (int i = 0; i < mini; i++)
        {
            stars[i].transform.parent.parent.gameObject.SetActive(true);

        }
    }

    private void starShow(GameObject starObj,bool isShow)
    {
        starObj.SetActive(isShow);
        starObj.transform.parent.parent.gameObject.SetActive(isShow);
    }
}
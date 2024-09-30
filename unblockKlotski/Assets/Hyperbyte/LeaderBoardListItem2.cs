using System.Collections;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace LeaderBoardSuperScrollView
{
    public class LeaderBoardListItem2 : MonoBehaviour
    {
        [SerializeField]
        private Sprite No1;
        [SerializeField]
        private Sprite No2;
        [SerializeField]
        private Sprite No3;


        [SerializeField]
        private Image NoImage;

        [SerializeField]
        private Text ImgNoTxt;

        [SerializeField]
        private Image selfImage;

        public Text mDescText;
        public Text mNameText;
        public Text mDescText2;  
      
        public void Init()
        {
           
        }


        public void SetItemData(ItemData itemData,int itemIndex)
        {
            NoImage.gameObject.SetActive(false);
            if (itemIndex == 0)
            {
                NoImage.sprite = No1;
                ImgNoTxt.text = "1";
                NoImage.gameObject.SetActive(true);

            }
            else if (itemIndex == 1)
            {
                NoImage.sprite = No2;
                ImgNoTxt.text = "2";
                NoImage.gameObject.SetActive(true);

            }
            else if (itemIndex == 2)
            {
                NoImage.sprite = No3;
                ImgNoTxt.text = "3";
                NoImage.gameObject.SetActive(true);
            }

            selfImage.gameObject.SetActive(false);
            if (itemData.playFabId == PlayFabAuthService.Instance.playFabId)
            {
                selfImage.gameObject.SetActive(true);
            }
     

            mDescText.text = (itemIndex+1).ToString();
            mNameText.text = itemData.mName;
            mDescText2.text = itemData.mDesc;
        }

        public void SetRankData(int Position, string DisplayName, int StatValue)
        {
            NoImage.gameObject.SetActive(false);
            if (Position == 0)
            {
                NoImage.sprite = No1;
                NoImage.gameObject.SetActive(true);
               
            }
            else if (Position == 1)
            {
                NoImage.sprite = No2;
                NoImage.gameObject.SetActive(true);

            }
            else if (Position == 2)
            {
                NoImage.sprite = No3;
                NoImage.gameObject.SetActive(true);
            }

            mDescText.text = (Position+1).ToString();
            mNameText.text = DisplayName;
            mDescText2.text = StatValue.ToString();
        }


    }
}

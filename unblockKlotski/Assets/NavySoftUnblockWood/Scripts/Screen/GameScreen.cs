using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopUpFrameWork;
using Hyperbyte;

namespace ScreenFrameWork {
    public class GameScreen : Screen
    {
 
        [SerializeField]
        private GameObject playingObj;
 
        [SerializeField]
        private MakeBoardLevel boardLevel;
    

        public override void Show(bool back, bool immediate)
        {
           
            SetUpBoard();
            base.Show(back, immediate);
        }
        public override void Hide(bool back, bool immediate)
        {
          
            VisibleBoardBG(false);
            if(PlayingManager.instance!=null)
            PlayingManager.instance.HideAllBlock();
            if (PopupManager.instance != null)
                PopupManager.instance.HideAllPopup();
            base.Hide(back, immediate);
        }

        private void VisibleBoardBG(bool visible)
        {
            playingObj.SetActive(visible);
           
        }


        private void SetUpBoard()
        {
            InputManager.Instance.DisableTouch();
            BoosterManager.instance.IsMoveBeginGame = false;
            BoosterManager.instance.isCanSave = false;

            Timer.Schedule(this, .04f, () =>
            {
                VisibleBoardBG(true);
                if (GameManager.instance.isResumeGame)
                {
                    PlayingManager.instance.ResumeGame();
                    GameManager.instance.isResumeGame = false;
                }
                else
                {
                    PlayingManager.instance.PlayGame();
                }

            });
           
        }
    }
}

    // ©2020 - 2021 HYPERBYTE STUDIOS 
// All rights reserved
// Redistribution of this software is strictly not allowed.
// Copy of this software can be obtained from unity asset store only.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;
using UnityEngine.UI;
using Hyperbyte.Localization;
using Hyperbyte.Ads;
using TMPro;
using System;

namespace Hyperbyte
{
    /// <summary>
    /// This script is used to rescue game using coins or watching video.
    /// 道具使用中心
    /// </summary>
    public class ToolUseQueryScript : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] TextMeshProUGUI txtTitle;
        [SerializeField] TextMeshProUGUI txtContent;
        [SerializeField] RectTransform gemsIcon;
        [SerializeField] TextMeshProUGUI txtRescueGemAmount;
        [SerializeField] Button BtnRescueWithAds;
        [SerializeField] TextMeshProUGUI unlockcontent;
#pragma warning restore 0649

        public const string USE_HINT= "USE_HINT";
        public const string USE_UNLOCK = "USE_UNLOCK";
        public const string USE_SKIN = "USE_SKIN";

        bool attemptedUseToolWithGems = false;
        string toolVideoTag = "";

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            txtRescueGemAmount.text = ProfileManager.Instance.GetAppSettings().rescueGameGemsAmount.ToString();
        }

        private Action useUnlockCallback;
        public void SetToolReason(string toolsTag, Action callback=null,string unlockTitle=null)
        {
            unlockcontent.gameObject.SetActive(false);
            switch (toolsTag)
            {
                case USE_HINT:
                    txtTitle.SetTextWithTag("txtUseTool");
                    txtContent.SetFormattedTextWithTag("useHintTool", "1");
                    toolVideoTag = toolsTag;
                    break;

                case USE_UNLOCK:
                    txtTitle.SetTextWithTag("txtUseTool");
                    txtContent.SetFormattedTextWithTag("useUnlockTool", "1");
                    unlockcontent.gameObject.SetActive(true);
                    unlockcontent.text = unlockTitle;
                    toolVideoTag = toolsTag;
                    useUnlockCallback = callback;
                    break;
                case USE_SKIN:
                    txtTitle.SetTextWithTag("txtUseTool");
                    txtContent.SetFormattedTextWithTag("useSkinTool", "1");
                    toolVideoTag = toolsTag;
                    break;
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            /// Pauses the game when it gets enabled.
          ////  PlayingController.instance.PauseGame(true);
            AdManager.OnRewardedAdRewardedEvent += OnRewardedAdRewarded;
            AdManager.OnRewardedLoadedEvent += OnRewardedLoaded;
            UIController.Instance.EnableCurrencyBalanceButton();

            updateRewardButton();
        }

        private void updateRewardButton()
        {
            if (AdManager.Instance.IsRewardedAvailable())
            {
                BtnRescueWithAds.GetComponent<CanvasGroup>().alpha = 1.0F;
                BtnRescueWithAds.interactable = true;
            }
            else
            {
                BtnRescueWithAds.GetComponent<CanvasGroup>().alpha = 0.5F;
                BtnRescueWithAds.interactable = false;
            }
        }

            /// <summary>
            /// This function is called when the behaviour becomes disabled or inactive.
            /// </summary>
        private void OnDisable()
        {
          ////  PlayingController.instance.PauseGame(false);
            /// Resumes the game when it gets enabled.
            AdManager.OnRewardedAdRewardedEvent -= OnRewardedAdRewarded;
            AdManager.OnRewardedLoadedEvent -= OnRewardedLoaded;
            attemptedUseToolWithGems = false;
            UIController.Instance.DisableCurrencyBalanceButton();
           
        }

        void OnRewardedLoaded()
        {
            updateRewardButton();
        }


        /// <summary>
        /// Will rescue game after showing rewarded video ad.
        /// 点击广告继续
        /// </summary>
        public void OnContinueWithWatchVideoButtonPressed()
        {
            if (InputManager.Instance.canInput()) {
                UIFeedback.Instance.PlayButtonPressEffect();
                ShowRewardedToUseTool();
            }
        }

        /// <summary>
        /// Will rescue game with gems.
        /// 点击钻石继续
        /// </summary>
        public void OnContinueWithGemsButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
                UIFeedback.Instance.PlayButtonPressEffect();
                if (CurrencyManager.Instance.DeductGems(ProfileManager.Instance.GetAppSettings().rescueGameGemsAmount))
                {   //减少砖石
                    //播放砖石动画
                    UIController.Instance.PlayDeductGemsAnimation(gemsIcon.position, 0.1F);
                    Invoke("UseToolWithGems", 1.5F);
                }
                else
                {
                    //砖石不够，弹出商店框
                    attemptedUseToolWithGems = true;
                    //Will open shop if not having enough gems.
                    UIController.Instance.shopScreen.Activate();
                }
            }
        }

    

        /// <summary>
        /// Closes pause screen and resumes gameplay.
        /// </summary>
        public void OnCloseButtonPressed()
        {
            if (InputManager.Instance.canInput())
            {
              
                UIFeedback.Instance.PlayButtonPressEffect();
                gameObject.Deactivate();
            }
        }

        void ShowRewardedToUseTool()
        {
            if (AdManager.Instance.IsRewardedAvailable()) {
                AdManager.Instance.ShowRewardedWithTag(toolVideoTag);
            }
        }



        /// <summary>
        /// Will start/continnue game with rescue successful.
        /// </summary>
        void UseToolWithGems()
        {
            InputManager.Instance.DisableTouchForDelay(1F);
            ////   BoosterController.instance.AddHint(int.Parse(toolVideoTag));
            ///
            DoUseTheTool();
            gameObject.Deactivate();
        }

        /// <summary>
        ///  Rewarded Ad Successful.see
        /// </summary>
        void OnRewardedAdRewarded(string watchVidoTag)
        {
            if (watchVidoTag == toolVideoTag)
            {
                ////BoosterController.instance.AddHint(int.Parse(toolVideoTag));
                DoUseTheTool();
                gameObject.Deactivate();
            }
        }

        //这里可采取抛出事件的做法，事件中心-中枢：解耦
        void DoUseTheTool()
        {
            switch (toolVideoTag)
            {
                case USE_HINT:
                    PlayingManager.instance.HintGame();
                    break;
                case USE_UNLOCK:
                    if (useUnlockCallback != null)
                    {
                        useUnlockCallback();
                        useUnlockCallback = null;
                    }
                    break;
                case USE_SKIN:

                    break;
            }
        }

        /// <summary>
        ///  Not in use. THis method can be called if rescue should be executed on sufficient balance received.
        /// </summary>
        public void ReattemptRescueWithGems()
        {
            if (attemptedUseToolWithGems)
            {
                if (CurrencyManager.Instance.DeductGems(ProfileManager.Instance.GetAppSettings().rescueGameGemsAmount))
                {
                    UIController.Instance.PlayDeductGemsAnimation(gemsIcon.position, 0.1F);
                    Invoke("UseToolWithGems", 1.5F);
                }
            }
        }
    }
}


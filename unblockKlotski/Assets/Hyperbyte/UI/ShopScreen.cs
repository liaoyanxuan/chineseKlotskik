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

using Hyperbyte.Ads;
using Hyperbyte.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hyperbyte
{
	public class ShopScreen : MonoBehaviour 
	{
		#pragma warning disable 0649
		[SerializeField] RectTransform mainContentRect;
		[SerializeField] GameObject btnRemoveAds;
        [SerializeField] GameObject coin1;
        [SerializeField] GameObject coin2;
        [SerializeField] GameObject coin3;
        [SerializeField] GameObject coin4;
        [SerializeField] CanvasGroup btnWatchVideo;
		[SerializeField] TextMeshProUGUI txtWatchVideoReward;
#pragma warning restore 0649


		string FreeGemsVideoTag = "FreeGems";
		Vector2 currentContentSize;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		void Awake() {
			currentContentSize = mainContentRect.sizeDelta;
		}
		/// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
		private void OnEnable() 
		{
            UIController.Instance.EnableCurrencyBalanceButton();
            IAPManager.OnPurchaseSuccessfulEvent += OnPurchaseSuccessful;
            AdManager.OnRewardedLoadedEvent += OnRewardedLoaded;
            AdManager.OnRewardedClosedEvent += OnRewardedClosed;
            UpdateShopScreen();

        }

		/// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
		private void OnDisable() {
			// Don't hide gems button if rescue screen is open.
            UIController.Instance.Invoke("DisableCurrencyBalanceButton",0.1F);
            IAPManager.OnPurchaseSuccessfulEvent -= OnPurchaseSuccessful;
            AdManager.OnRewardedLoadedEvent -= OnRewardedLoaded;
            AdManager.OnRewardedClosedEvent -= OnRewardedClosed;

           
        }

        void OnRewardedLoaded()
        {
            UpdateShopScreen();
        }

        void OnRewardedClosed(string watchVidoTag)
        {
            UpdateShopScreen();
        }

        /// <summary>
        /// Close button click listener.
        /// </summary>
        public void OnCloseButtonPressed() {
			if(InputManager.Instance.canInput()) {
				UIFeedback.Instance.PlayButtonPressEffect();
				gameObject.Deactivate();
			}
		}

		/// <summary>
		/// Purchase button click listener.
		/// </summary>
		public void OnPurhcaseButtonClicked() {
			if(InputManager.Instance.canInput()) {
				UIFeedback.Instance.PlayButtonPressEffect();
				//gameObject.Deactivate();
			}
		}

		/// <summary>
		/// Purchase button click listener.
		/// </summary>
		public void OnGetFreeGemsButtonPressed() {
			if(InputManager.Instance.canInput()) {
				UIFeedback.Instance.PlayButtonPressEffect();
				AdManager.Instance.ShowRewardedWithTag(FreeGemsVideoTag);
			}
		}

		/// <summary>
        /// Purchase Rewards will be processed from here. You can adjust your code based on your requirements.
        /// </summary>
        /// <param name="productInfo"></param>
		void OnPurchaseSuccessful(ProductInfo productInfo) {
			Invoke("UpdateShopScreen", 0.2F);
		}

		void UpdateShopScreen() 
		{
			txtWatchVideoReward.text = string.Format(LocalizationManager.Instance.GetTextWithTag("txtGems_FR"), ProfileManager.Instance.GetAppSettings().watchVideoRewardAmount);
			if(AdManager.adobjectInit==false || !AdManager.Instance.IsRewardedAvailable()) {
				btnWatchVideo.alpha = 0.5F;
				btnWatchVideo.interactable = false;
			} else {
				btnWatchVideo.alpha = 1.0F;
				btnWatchVideo.interactable = true;
			}

 			if (ProfileManager.Instance.IsAppAdFree()) {
				btnRemoveAds.SetActive(false);
			////	mainContentRect.sizeDelta = new Vector2(currentContentSize.x, currentContentSize.y - btnRemoveAds.GetComponent<RectTransform>().sizeDelta.y);
			} else {
				btnRemoveAds.SetActive(true);
			}


#if UNITY_IOS || BYTEDGAME || UNITY_ANDROID
			btnRemoveAds.SetActive(false);
            coin1.SetActive(false);
            coin2.SetActive(false);
            coin3.SetActive(false);
            coin4.SetActive(false);
          ////  mainContentRect.sizeDelta = new Vector2(currentContentSize.x, currentContentSize.y - btnRemoveAds.GetComponent<RectTransform>().sizeDelta.y*5);

#endif
        }
    }
}

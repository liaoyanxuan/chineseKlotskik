// ©2019 - 2020 HYPERBYTE STUDIOS LLP
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

using System;
using UnityEngine;

#if HB_ADMOB
using GoogleMobileAds.Api;
using System;
#endif

namespace Hyperbyte.Ads
{
    /// <summary>
    /// This class component will be added to game dynamically if Google Ads is selected as active ad network.
    /// All the callbacks will be forwarded to ad manager.
    /// </summary>
    public class GoogleMobileAdsManager : AdHelper
    {
        GoogleMobileAdsSettings settings;

        #if HB_ADMOB
        private BannerView bannerView;
        private InterstitialAd interstitial;
        private RewardedAd rewardedAd;
        #endif

        /// <summary>
        /// Initialized the ad network.
        /// </summary>
        public override void InitializeAdNetwork()
        {
            settings = (GoogleMobileAdsSettings)(Resources.Load("AdNetworkSettings/GoogleMobileAdsSettings"));

            #if HB_ADMOB
            MobileAds.SetiOSAppPauseOnBackground(true);
            #endif

            Invoke("StartLoadingAds", 2F);
        }

        /// <summary>
        /// Loads ads after initialization.
        /// </summary>
        public void StartLoadingAds()
        {
            RequestBannerAds();
            RequestInterstitial();
            RequestRewarded();
        }

        // Requests banner ad.        
        public void RequestBannerAds()
        {
           
        }

        // Requests intestitial ad.
        public void RequestInterstitial()
        {
            
        }

        // Requests rewarded ad.
        public void RequestRewarded()
        {
           
        }

        #if HB_ADMOB
        private AdRequest CreateAdRequest()
        {
            return null;

        }
        #endif
        
        // Shows banner ad.
        public override void ShowBanner()
        {
            
        }
    
        // Hides banner ad.
        public override void HideBanner()
        {
            
        }

        // Check if interstial ad ready to show.
        public override bool IsInterstitialAvailable()
        {
           
            return false;
        }
        
        // Shows interstitial ad if available.
        public override void ShowInterstitial(Action<bool> closeCallBack)
        {
           
        }

        // Checks if rewarded ad ready to show.
        public override bool IsRewardedAvailable()
        {
            
            return false;
        }

        // Shows rewarded ad if loaded.
        public override void ShowRewarded()
        {
           
        }

        #if HB_ADMOB
        #region Banner callback handlers
        // Banner ad  event callbacks.
        public void HandleBannerAdLoaded(object sender, EventArgs args) {
            AdManager.Instance.OnBannerLoaded();
        }

        public void HandleBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
            AdManager.Instance.OnBannerLoadFailed("");
        }

        public void HandleBannerAdOpened(object sender, EventArgs args) {
        }

        public void HandleBannerAdClosed(object sender, EventArgs args) {
        }

       
        #endregion

        #region Interstitial callback handlers
        // Interstitial ad event callbacks.
        public void HandleInterstitialLoaded(object sender, EventArgs args) {
            AdManager.Instance.OnInterstitialLoaded();
        }

        public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
            AdManager.Instance.OnInterstitialLoadFailed("");
        }

        public void HandleInterstitialOpened(object sender, EventArgs args) {
            AdManager.Instance.OnInterstitialShown();
        }

        public void HandleInterstitialClosed(object sender, EventArgs args) {
            MonoBehaviour.print("HandleInterstitialClosed event received");
            RequestInterstitial();
            AdManager.Instance.OnInterstitialClosed();
        }

    

        #endregion

        #region RewardedAd callback handlers
        // Rewarded ad event callbacks.
        public void HandleRewardedAdLoaded(object sender, EventArgs args) {
            AdManager.Instance.OnRewardedLoaded();
        }

        public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
            AdManager.Instance.OnRewardedLoadFailed("");
        }

        public void HandleRewardedAdOpening(object sender, EventArgs args) {
           AdManager.Instance.OnRewardedShown();
        }

        public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)  {
        }

        public void HandleRewardedAdClosed(object sender, EventArgs args)
        {
            RequestRewarded();
            AdManager.Instance.OnRewardedClosed();
        }

        public void HandleUserEarnedReward(object sender, Reward args) {
            string type = args.Type;
            double amount = args.Amount;
            AdManager.Instance.OnRewardedAdRewarded();
        }
        #endregion
        #endif
    }
}

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

using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

namespace Hyperbyte 
{   
    /// <summary>
    /// This script is attached to purchase success popup.
    /// </summary>
    public class PlayFabLogin : MonoBehaviour
    {

        public Button PlayAsGuestButton;

        // Settings for what data to get from playfab on login.
        public GetPlayerCombinedInfoRequestParams InfoRequestParams;

        // Reference to our Authentication service
        private PlayFabAuthService _AuthService = PlayFabAuthService.Instance;


        [SerializeField]
        private GameObject loginPanel;

        [SerializeField]
        private GameObject inputDisplayNamePanel;

        [SerializeField]
        private GameObject confirmDisplayNamePanel;

        public void Awake()
        {
            _AuthService.RememberMe = true;
        }

            /// <summary>
            /// This function is called when the behaviour becomes enabled or active.
            /// </summary>
        private void OnEnable() {
            loginPanel.gameObject.SetActive(true);
            inputDisplayNamePanel.gameObject.SetActive(false);
            confirmDisplayNamePanel.gameObject.SetActive(false);
        }


        public void Start()
        {
            PlayFabAuthService.OnLoginSuccess += OnLoginSuccess;
            PlayFabAuthService.OnPlayFabError += OnPlayFaberror;

            // Set the data we want at login from what we chose in our meta data.
            _AuthService.InfoRequestParams = InfoRequestParams;
        }


        /// <summary>
        /// Login Successfully - Goes to next screen.
        /// </summary>
        /// <param name="result"></param>
        private void OnLoginSuccess(PlayFab.ClientModels.LoginResult result)
        {
            UIController.Instance.HideLoadingTip();
            Debug.LogFormat("Logged In as: {0}", result.PlayFabId);

            string UserNameText = result.InfoResultPayload.AccountInfo.Username ?? result.PlayFabId;

            Debug.LogFormat("Logged In UserName: {0}", UserNameText);

            string diaplayName = null;
            if (result.InfoResultPayload.AccountInfo.TitleInfo != null)
            {
                diaplayName = result.InfoResultPayload.AccountInfo.TitleInfo.DisplayName;
            }

            PlayFabAuthService.Instance.playFabId = result.PlayFabId;

            if (diaplayName == null)
            {
                //显示输入昵称
                showInputDisplayName();
            }
            else
            {
                PlayFabAuthService.Instance.tempDisplayNameId = diaplayName;
                PlayFabAuthService.Instance.RememberMeDisplayNameId = diaplayName;
                //请求显示排行榜
                gameObject.Deactivate();

                LeaderboardManager.getInstance().Authenticate(LeaderboarMode.SHOW_BOARD);

            }
        }


        /// <summary>
        /// Error handling for when Login returns errors.
        /// </summary>
        /// <param name="error"></param>
        private void OnPlayFaberror(PlayFabError error)
        {
            PlayFabAuthService.Instance.tempDisplayNameId = null;
            PlayFabAuthService.Instance.playFabId = null;
            UIController.Instance.HideLoadingTip();
            //There are more cases which can be caught, below are some
            //of the basic ones.
            switch (error.Error)
            {
                case PlayFabErrorCode.InvalidEmailAddress:
                case PlayFabErrorCode.InvalidPassword:
                case PlayFabErrorCode.InvalidEmailOrPassword:
                    break;

                case PlayFabErrorCode.AccountNotFound:
                    return;
                default:
                    break;
            }

            //Also report to debug console, this is optional.
            Debug.Log(error.Error);
            Debug.LogError(error.GenerateErrorReport());

            UIController.Instance.ShowAlertTip("排行榜登陆失败：" + error.GenerateErrorReport() + " " + error.Error);
        }

        private void showInputDisplayName()
        {
            loginPanel.SetActive(false);
            inputDisplayNamePanel.SetActive(true);
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
        /// Ok button click listener.
        /// </summary>
        public void OnOkButtonPressed() {
			if(InputManager.Instance.canInput()) {
                UIFeedback.Instance.PlayButtonPressEffect();
                _AuthService.Authenticate(Authtypes.Silent);
                UIController.Instance.ShowLoadingTip();
            }
		}
    }
}

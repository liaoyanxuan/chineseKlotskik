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

namespace Hyperbyte
{
	[RequireComponent(typeof(Button))]
	public class ThemeIconButton : MonoBehaviour 
	{
		Button themeSettingButton;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		private void Awake() {
			themeSettingButton = GetComponent<Button>();	
		}

		void Start()
		{
			if (!ThemeManager.Instance.UIThemeEnabled)
			{
				themeSettingButton.gameObject.SetActive(false);
			}

		}

		/// <summary>
		/// This function is called when the behaviour becomes enabled or active.
		/// </summary>
		private void OnEnable() {
			themeSettingButton.onClick.AddListener(OnThemeButtonClicked);
		}

		/// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
		private void OnDisable() {
			themeSettingButton.onClick.RemoveListener(OnThemeButtonClicked);
		}

		void OnThemeButtonClicked()
		{
			 if (InputManager.Instance.canInput())
            {
                InputManager.Instance.DisableTouchForDelay();
                UIFeedback.Instance.PlayButtonPressEffect();
                UIController.Instance.selectThemeScreen.Activate();
            }
		}
	}
}
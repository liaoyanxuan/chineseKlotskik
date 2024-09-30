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
	public class IntroduceButton : MonoBehaviour 
	{
		Button btnIntroduce;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		private void Awake() {
			btnIntroduce = GetComponent<Button>();	
		}

		/// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
		private void OnEnable() {
			btnIntroduce.onClick.AddListener(OnIntroduceButtonClicked);
		}

		/// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
		private void OnDisable() {
			btnIntroduce.onClick.RemoveListener(OnIntroduceButtonClicked);
		}

		void OnIntroduceButtonClicked() {
			if(InputManager.Instance.canInput()) {
				UIFeedback.Instance.PlayButtonPressEffect();
				UIController.Instance.introduceScreen.Activate();
			}
		}
	}
}
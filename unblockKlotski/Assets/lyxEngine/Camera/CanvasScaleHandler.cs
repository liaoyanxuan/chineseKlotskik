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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hyperbyte
{
	public class CanvasScaleHandler : MonoBehaviour 
	{
		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		//竖屏游戏
		public static float devHeightRatio;




		[SerializeField]
		private float devHeight = 1920f; //设计分辨率

		[SerializeField]
		private float devWidth = 1080f; //设计分辨率

		private static RectTransform rectTrans;

		private void Awake() {

			float screenAspect = 0.0F;


			if (Screen.height > Screen.width) {
				screenAspect = (((float)Screen.height) / ((float)Screen.width));
			} else {
				screenAspect = (((float)Screen.width) / ((float)Screen.height));
			}

			if (screenAspect < 1.75F) {
				GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0F;
				devHeightRatio = ((float)Screen.height) / 1334;
			} else {
				GetComponent<CanvasScaler>().matchWidthOrHeight = 0F;

				devHeightRatio = ((float)Screen.height) / (devWidth * screenAspect);
				//height1920Ratio = (devWidth * screenAspect)/1920f;

			}

			rectTrans = this.GetComponent<RectTransform>();

		}


		public static float ratioWidth
		{
			get
			{

				return rectTrans.sizeDelta.x;
			}
		}

		public static float ratioHeight
		{
			get
			{

				return rectTrans.sizeDelta.y;
			}
		}

		private static float _safeAreaHeight = 0f;
		public static float safeAreaHeight
		{
            get 
			{
				if (_safeAreaHeight == 0f)
				{
					_safeAreaHeight = Screen.safeArea.height;
				}
				return _safeAreaHeight;
			}

        }


		private static float _safeAreaWeight = 0f;
		public static float safeAreaWeight
		{
			get
			{
				if (_safeAreaWeight == 0f)
				{
					_safeAreaWeight = Screen.safeArea.width;
				}
				return _safeAreaWeight;
			}

		}

		/*
		private void Update()
		{

			float screenAspect = 0.0F;


			if (Screen.height > Screen.width)
			{
				screenAspect = (((float)Screen.height) / ((float)Screen.width));
			}
			else
			{
				screenAspect = (((float)Screen.width) / ((float)Screen.height));
			}

			if (screenAspect < 1.75F)
			{
				GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0F;
				devHeightRatio = ((float)Screen.height) / 1334;
			}
			else
			{
				GetComponent<CanvasScaler>().matchWidthOrHeight = 0F;

				devHeightRatio = ((float)Screen.height) / (devWidth * screenAspect);
				//height1920Ratio = (devWidth * screenAspect)/1920f;

			}
		}
		*/
	}
}
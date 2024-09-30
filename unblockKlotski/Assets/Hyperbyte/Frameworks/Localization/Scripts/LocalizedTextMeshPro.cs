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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hyperbyte.Localization
{

   
    /// <summary>
    /// This script can be attached to any UI Text component with text tag.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTextMeshPro : MonoBehaviour
    {
        #pragma warning disable 0649
        [Tooltip("Assign Text tag containing localized text.")]
        [SerializeField] string txtTag;
        #pragma warning restore 0649
        
        TextMeshProUGUI thisText;

        [SerializeField]
        private int ChineseFontSize = 0;

       
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            thisText = GetComponent<TextMeshProUGUI>();

            if (txtTag == null)
            {
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes enabled or active.
        /// </summary>
        private void OnEnable()
        {
            LocalizationManager.OnLocalizationInitializedEvent += OnLocalizationInitialized;
            LocalizationManager.OnLanguageChangedEvent += OnLanguageChanged;

            LocalizeContent();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            LocalizationManager.OnLocalizationInitializedEvent -= OnLocalizationInitialized;
            LocalizationManager.OnLanguageChangedEvent -= OnLanguageChanged;
        }


        /// <summary>
        /// Event callback on localization initializes.
        /// </summary>
        void OnLocalizationInitialized(LocalizedLanguage lang, bool isLocalizationSupported)
        {
            if (isLocalizationSupported)
            {
                LocalizeContent();
            }
        }

        /// <summary>
        /// Event callback on language change.
        /// </summary>
        void OnLanguageChanged(LocalizedLanguage lang)
        {
            LocalizeContent();
        }

        void LocalizeContent()
        {
            if (LocalizationManager.Instance.hasLanguageChanged)
            {
                thisText.SetTextWithTag(txtTag);
            }

            if (LocalizationManager.Instance.GetCurrentLanguage() != null)
            {
                /*
                if (LocalizationManager.Instance.GetCurrentLanguage().languageCode == "CN")
                {
                    if (ChineseFontSize != 0)
                    {
                        thisText.enableAutoSizing = false;
                        thisText.fontSize = ChineseFontSize;
                    }
                }
                */

                if (ChineseFontSize != 0)
                {
                    thisText.enableAutoSizing = false;
                    thisText.fontSize = ChineseFontSize;
                }
            }

        }
    }
}

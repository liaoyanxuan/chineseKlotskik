using System.Collections;
using System.Collections.Generic;
using Hyperbyte;
using UnityEngine;

public class IntroduceGameScreen : MonoBehaviour
{
	public void OnCloseButtonPressed()
	{
		if (InputManager.Instance.canInput())
		{
			UIFeedback.Instance.PlayButtonPressEffect();
			gameObject.Deactivate();
		}
	}
}

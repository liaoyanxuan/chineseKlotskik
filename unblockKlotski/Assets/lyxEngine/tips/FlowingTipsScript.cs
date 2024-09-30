using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.DesignPatterns;

public class FlowingTipsScript : SingletonPattern<FlowingTipsScript>
{

	[SerializeField]
	private Text flowingTips;
	[SerializeField]
	private RectTransform flowingTipRectTransform;
	// Use this for initialization

	private const float targetAlpha=0f;
	private const float startAlpha = 1f;
	private const float mTargetValue=380f;
	private const float SMOOTH_TIME = 2F;

	private bool mNeedMove = false;
	private float mOnAlphSpeed = 0f;
	private float mMoveSpeed = 0f;

	private Vector2 originalPos=new Vector2 (0, 0);

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (mNeedMove)
		{
			if (Mathf.Abs(flowingTipRectTransform.anchoredPosition.y - mTargetValue) < 0.01f)
			{
				flowingTipRectTransform.anchoredPosition = new Vector2(0, mTargetValue);
				changeImageAlpha(flowingTips, targetAlpha);
				mNeedMove = false;
				this.gameObject.SetActive (false);
				return;
			}

			float onAlpha = Mathf.SmoothDamp(flowingTips.color.a, targetAlpha, ref mOnAlphSpeed, SMOOTH_TIME);
			changeImageAlpha(flowingTips, onAlpha);


			float posY = Mathf.SmoothDamp(flowingTipRectTransform.anchoredPosition.y, mTargetValue, ref mMoveSpeed, SMOOTH_TIME);
			flowingTipRectTransform.anchoredPosition = new Vector2(0, posY);
		}
	}

	public void showTips(string flowingText)
	{
		this.gameObject.SetActive (true);
		mNeedMove = true;
		mOnAlphSpeed = 0f;
		mMoveSpeed = 0f;

        flowingTips.color = Color.black;

        flowingTips.text = flowingText;
		changeImageAlpha (flowingTips,startAlpha);
		flowingTipRectTransform.anchoredPosition = originalPos;


	}

	private void changeImageAlpha(Text txt,float targetAlpha) 
	{
		Color color = txt.color;
		color.a = targetAlpha;
		txt.color = color;
	}
}

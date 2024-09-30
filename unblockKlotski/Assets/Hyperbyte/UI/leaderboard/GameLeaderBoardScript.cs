using Hyperbyte;
using LeaderBoardSuperScrollView;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLeaderBoardScript : MonoBehaviour
{

    [SerializeField]
    public LeaderBoardListItem2 myrank;
    /// <summary>
    /// Close button click listener.
    /// </summary>
    public void OnCloseButtonPressed()
    {
        if (InputManager.Instance.canInput())
        {
            UIFeedback.Instance.PlayButtonPressEffect();
            gameObject.Deactivate();
        }
    }

    public void SetMyRankData(int Position, string DisplayName, int StatValue)
    {
   
        myrank.SetRankData( Position,  DisplayName,  StatValue);
    }

}

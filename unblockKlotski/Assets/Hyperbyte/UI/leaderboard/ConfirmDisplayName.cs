using Hyperbyte;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDisplayName : MonoBehaviour
{

    [SerializeField]
    private InputDisplayName inputDisplayName;
    // Start is called before the first frame update
    private InputField nameInputField;

   
    void Awake()
    {
        nameInputField = GetComponentInChildren<InputField>();
    }

    public void setInputName(string inputName)
    {
        nameInputField.text = inputName;
    }


    public void ConfirmtDisplayNameClose()
    {
        this.gameObject.SetActive(false);
        inputDisplayName.gameObject.SetActive(true);
    }

    public void ConfirmtDisplayNameOK()
    {
        if (InputManager.Instance.canInput())
        {
            string nameInput = nameInputField.text.Trim();
            if (string.IsNullOrEmpty(nameInput))
            {
                Toast.instance.ShowMessage("请输入游戏昵称");
            }
            else
            {
                SubmitNameButton(nameInput);
            }
          
        }
    }

    public void SubmitNameButton(string nameInput)
    {
      
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameInput,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnPlayFaberror);

    }

    private void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        this.gameObject.SetActive(false);
        UIController.Instance.leaderBoardLogin.Deactivate();
        Debug.Log("Update display name!"+ result.DisplayName);

        PlayFabAuthService.Instance.tempDisplayNameId = result.DisplayName;
        PlayFabAuthService.Instance.RememberMeDisplayNameId = result.DisplayName;
        PlayFabAuthService.Instance.firstDisplay = true;

        //请求显示排行榜
        LeaderboardManager.getInstance().Authenticate(LeaderboarMode.SHOW_BOARD);
    }

    /// <summary>
    /// Error handling for when Login returns errors.
    /// </summary>
    /// <param name="error"></param>
    private void OnPlayFaberror(PlayFabError error)
    {
        this.gameObject.SetActive(false);
        UIController.Instance.leaderBoardLogin.Deactivate();
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

     
        UIController.Instance.ShowAlertTip("创建昵称失败："+ error.Error);
    }
}

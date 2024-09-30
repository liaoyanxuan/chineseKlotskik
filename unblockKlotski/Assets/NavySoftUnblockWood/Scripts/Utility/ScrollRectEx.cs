using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectEx : ScrollRect
{
    [SerializeField]
    private GAME_MODE_ID m_GameModeId;
    // private int m_GameModeId;


    /// <summary>
    /// The behavior to use when the content moves beyond the scroll rect.
    /// </summary>
    public GAME_MODE_ID gameModeId
    {
        get
        {
            return m_GameModeId;
        }
        set
        {
            m_GameModeId = value;
        }
    }

}

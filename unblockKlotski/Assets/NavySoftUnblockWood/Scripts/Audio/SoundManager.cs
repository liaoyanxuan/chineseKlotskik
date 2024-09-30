using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField]
    private AudioSource audioSound;
    [SerializeField]
    private AudioSource starEarn;
    [SerializeField]
    private AudioSource winGame;

    [SerializeField]
    private AudioSource firstBlockPlace;

    [SerializeField]
    private AudioSource xiuflash;

    [SerializeField]
    private AudioSource BGM;
    [SerializeField]
    private AudioClip button;
    [SerializeField]
    private AudioClip clipWinGame;
    [SerializeField]
    private AudioClip clipBlockTouch;
    [SerializeField]
    private AudioClip clipBlockPlace;

    [SerializeField]
    private AudioClip firstCollision;

    [SerializeField]
    private AudioSource secondCollision;

    [SerializeField]
    private AudioClip starComplete;

    [SerializeField]
    private List<Image> buttonSound = new List<Image>();
    [SerializeField]
    private List<Image> buttonMusic= new List<Image>();
    [SerializeField]
    private Sprite musicOn;
    [SerializeField]
    private Sprite musicOff;
    [SerializeField]
    private Sprite soundOn;
    [SerializeField]
    private Sprite soundOff;

    private bool muteSound = false;
    private bool muteMusic = true; //默认不开音乐
    private void Awake()
    {
        instance = this;
 
    }

	private void Start()
	{
        if (PlayerPrefs.HasKey("Music") == false)
        {
            PlayerPrefs.SetInt("Music", 1);//默认不开音乐
        }
        muteMusic = PlayerPrefs.GetInt("Music") == 1 ? true : false;
        muteSound = PlayerPrefs.GetInt("Sound") == 1 ? true : false;

        BGM.volume = muteMusic?0:0.5f;

        audioSound.volume = muteSound ? 0 : 1;
      

        for (int i = 0; i < buttonMusic.Count; i++)
        {
            buttonMusic[i].sprite = muteMusic ? musicOff : musicOn;
        }

        for (int i = 0; i < buttonSound.Count; i++)
        {
            buttonSound[i].sprite = muteSound ? soundOff : soundOn;
        }

    }

    public void OnEventMusic()
    {
        muteMusic = !muteMusic;
        PlayerPrefs.SetInt("Music", (muteMusic ==true? 1 : 0));
        BGM.volume = muteMusic ? 0 : 0.5f;
        for (int i = 0; i < buttonMusic.Count; i++)
        {
            buttonMusic[i].sprite = muteMusic ? musicOff : musicOn;
        }
    }


    public void OnEventSound( )
    {

        muteSound = !muteSound;
        PlayerPrefs.SetInt("Sound", (muteSound == true ? 1 : 0));
        audioSound.volume = muteSound ? 0 : 1;
        for (int i = 0; i < buttonSound.Count; i++)
        {
            buttonSound[i].sprite = muteSound ? soundOff : soundOn;
        }


    }
    public void StarCompletedSound()
    {
        // audioSound.PlayOneShot(starComplete);
        if (audioSound.volume > 0.5f)
        {
            starEarn.Play();
        }
    }


    public void WinGameSound()
    {
      //  audioSound.PlayOneShot(clipWinGame);
        if (audioSound.volume > 0.5f)
        {
            winGame.Play();
        }
    }

    public void FirstBlockSound()
    {
        //  audioSound.PlayOneShot(clipWinGame);
        if (audioSound.volume > 0.5f)
        {
            firstBlockPlace.Play();
        }
    }

    public void xiuflashSound()
    {
        //  audioSound.PlayOneShot(clipWinGame);
        if (audioSound.volume > 0.5f)
        {
            xiuflash.Play();
        }
    }

    public void secondCollisionSound()
    {
        //  audioSound.PlayOneShot(clipWinGame);
        if (audioSound.volume > 0.5f)
        {
            secondCollision.Play();
        }
    }



    public void TouchBlock()
    {
        audioSound.PlayOneShot(clipBlockTouch);
    }
    public void BlockPlace(bool isCollision)
    {
        if (isCollision)
        {
            //audioSound.PlayOneShot(secondCollision);
            secondCollisionSound();
        }
        else
        {
            audioSound.PlayOneShot(clipBlockPlace);

        }
       
    }

    public void firstCollisionSound()
    {
        audioSound.PlayOneShot(firstCollision);
    }


    public void OnEventButtonSound()
	{
		audioSound.PlayOneShot(button);
	}

	 

	 

	 

	 
}

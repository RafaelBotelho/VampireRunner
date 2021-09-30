using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

public class UIManager : Singleton<UIManager>
{
    //Don't destroy the object when a scene is loaded
    private void Start()
    {
        DontDestroyOnLoad(gameObject); //Don't destroy the object when a scene is loaded
    }

    //Enable or disable a UI object
    public void EnableDisableUI(GameObject uiElemnt)
    {
        uiElemnt.SetActive(!uiElemnt.activeSelf); //Enable or disable the object
    }

    //Update a text
    public void UpdateText(Text textObject, string stringToBeDisplayed)
    {
        textObject.text = stringToBeDisplayed; //Update the text
    }

    //Animate a panel
    public void AnimatePanels(RectTransform panel, Vector3 position, float time, bool show)
    {
        //Check if the panel is to be shown or not
        if (show)
        {
            EnableDisableUI(panel.gameObject); //Enable the panel
            LeanTween.move(panel, position, time).setIgnoreTimeScale(true); //Animate the panel
        }
        else
        {
            Action action = () => EnableDisableUI(panel.gameObject); //Store the function to disable the panel
            LeanTween.move(panel, position, time).setIgnoreTimeScale(true).setOnComplete(action); //Move the panel
        }
    }

    //Save de audio settings
    public void SaveConfigs(float volumeBackgroung, float volumeSFX)
    {
        PlayerPrefs.SetFloat("CurrentVolumeBackground", volumeBackgroung); //Save the background audio setting
        PlayerPrefs.SetFloat("CurrentVolumeSFX", volumeSFX); //Save the sound effects audio setting
    }

    //Play a video
    public void PlayTutorial(VideoPlayer tutorialPlayer, VideoClip tutorialClip, Text tutorialTextDisplay, string tutorialText)
    {
        tutorialPlayer.clip = tutorialClip; //Pass the video clip to the video player
        tutorialPlayer.Play(); //Play the video
        tutorialTextDisplay.text = tutorialText; //Display the text
    }

    //Open the game page to the player rate the game
    public void RateGame()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.MuraGames.VampireRunner"); //Open the PlayStore Game URL
    }

    //Animate the HP display
    public void HPAnimation(RectTransform heart, bool loseHeart)
    {
        //Check if it's losing or recovering the HP
        if (loseHeart)
        {
            LeanTween.alpha(heart, 0, 1f).setIgnoreTimeScale(true); //Make the heart invisible
        }
        else
        {
            LeanTween.alpha(heart, 1, 1f).setIgnoreTimeScale(true); //Make the heart Visible
        }
    }
}

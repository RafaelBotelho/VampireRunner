using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<VideoClip> tutorialClips; //List of tutorial clips
    [SerializeField] private List<string> tutorialTexts; //List of texts displayed at the tutorial
    [SerializeField] private Button nextButton; //Reference to the button that will show the next clip
    [SerializeField] private Button previousButton; //Reference to the button that will show the previous clip
    [SerializeField] private Sprite tutorialConcluded; //Sprite that will change the next button when all the clips have been played
    [SerializeField] private AudioClip clickSFX; //Audio clip that will be played when the a button is clicked
    [SerializeField] private VideoPlayer VideoPlayer; //Reference to the video player
    [SerializeField] private Text textDisplay; //Reference to the text UI element
    [SerializeField] private int currentTutorial = 0; //Index of the current clip being played

    //Play the first tutorial and disable the previous clip button
    private void Start()
    {
        UIManager.Instance.PlayTutorial(VideoPlayer, tutorialClips[currentTutorial], textDisplay, tutorialTexts[currentTutorial]); //Call the UI Manager method to display the first tutorial
        previousButton.gameObject.SetActive(false); //Disable the previous clip button
    }

    //Increment the current clip index, check if there is an avaliable clip, play next clip, activate the previous clip button, go to the game scene when there's no avaliable clips
    public void PlayNextClip()
    {
        currentTutorial++; //Increment the current clip index

        //Check if there is an avaliable clip
        if (currentTutorial <= (tutorialClips.Count -1))
        {
            AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
            UIManager.Instance.PlayTutorial(VideoPlayer, tutorialClips[currentTutorial], textDisplay, tutorialTexts[currentTutorial]); //Display the next tutorial

            //Check if the previous clip button is active
            if (previousButton.gameObject.activeSelf == false)
            {
                previousButton.gameObject.SetActive(true); //Activate the previous clip button
            }

            //Check if the current clip is the last one on the list
            if (currentTutorial == (tutorialClips.Count -1))
            {
                nextButton.image.sprite = tutorialConcluded; //Change the next button sprite to the tutorial concluded sprite
            }
        }
        else
        {
            GoToGame(); //Go to the game scene
        }
    }

    //Decrement the current clip index, play the previous clip, disable the previous clip button if the current clip is the first one
    public void PlayPreviousClip()
    {
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
        currentTutorial--; //Decrement the current clip index
        UIManager.Instance.PlayTutorial(VideoPlayer, tutorialClips[currentTutorial], textDisplay, tutorialTexts[currentTutorial]); //Display the next tutorial

        //Check if the current clip is the last one
        if (currentTutorial == 0)
        {
            previousButton.gameObject.SetActive(false); //Disable the previous clip button
        }
    }

    //Method that will be called when the button play is pressed
    public void GoToGame()
    {
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
        LoadingManager.Instance.Unloadlevel("Tutorial"); //Call the Loading Manager method to unload the main menu scene
        LoadingManager.Instance.LoadLevel("Game"); //Call the Loading Manager method to load the game scene
    }
}

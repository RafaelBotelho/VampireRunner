using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.Collections.Generic;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private Slider backgroundVolume; //Reference to the slider that provide the background volume value 
    [SerializeField] private Slider sfxVolume; //Reference to the slider that provide the sound effect volume value 
    [SerializeField] private RectTransform pauseSettingsPanel; //Reference to the right pause panel
    [SerializeField] private RectTransform pauseOptionsPanel; //Reference to the left pause panel
    [SerializeField] private RectTransform continuePanel; //Reference to to top continue panel
    [SerializeField] private RectTransform endRunPanel; //Reference to the bottom continue panel
    [SerializeField] private RectTransform gameoverScore; //Reference to the top game over Panel
    [SerializeField] private RectTransform gameoverOptions; //Reference to the bottom game over Panel
    [SerializeField] private Button continueBloodButton; //Reference to the continue paying blood button
    [SerializeField] private Button pauseButton; //Reference to the pause button
    [SerializeField] private AudioClip clickSFX; //Audio clip that will be played when the a button is clicked
    [SerializeField] private Text continueTimer; //Reference to  the continue timer text
    [SerializeField] private Text distanceScore; //Reference to the distance score text element
    [SerializeField] private Text distanceGameOverScore; //Reference to the distance score displayed at the game over panel
    [SerializeField] private Text bestDistanceGameOverScore; //Reference to the best distance score displayed at the game over panel
    [SerializeField] private Text distacePausedScore; //Reference to the current distance score displayed at the pause panel
    [SerializeField] private Text bloodCount; //Reference to the blood score text element
    [SerializeField] private Text bloodGameOverScore; //Reference to the blood score displayed at the game over pane
    [SerializeField] private Text BestBloodScoreInOneRun; //Reference to the best blood score displayed at the game over panel
    [SerializeField] private Text bloodPausedScore; //Reference to the current blood score displayed at the pause panel
    [SerializeField] private Text initialTimer; //Reference to  the initial timer text
    [SerializeField] private int timerEndRun; //Time it need to wait to end the run
    [SerializeField] private int necessaryBlood; //Necessary amount of blood to pay to continue the run
    [SerializeField] private List<RectTransform> hpAnimator; //List of animators of the HP indicators
    private Vector3 pauseSettingsInitialPos; //Pause panel right side initial position
    private Vector3 pauseOptionsInitialPos; //Pause panel  left side initial position
    private Vector3 continuelPanelInitialPos; //Continue panel top side initial position
    private Vector3 EndRunInitialPos; //Continue panel bottom side initial position
    private Coroutine coroutine; //Timer coroutine

    //Set the panels initial positions at the start of the scene
    private void Start()
    {
        continuelPanelInitialPos = continuePanel.anchoredPosition; //Set the continue panel top side initial position
        EndRunInitialPos = endRunPanel.anchoredPosition; //Set the continue panel bottom side initial position
        pauseSettingsInitialPos = pauseSettingsPanel.anchoredPosition; //Set the pause panel  left side initial position
        pauseOptionsInitialPos = pauseOptionsPanel.anchoredPosition; //Set the pause panel right side initial position
        backgroundVolume.value = AudioManager.Instance.BackgroundVolume; //Set the volume to the background slider
        sfxVolume.value = AudioManager.Instance.SfxVolume; //Set the volume to the sound effects slider
        UpdateBlood(); //Set the blood count
    }

    //Subscribe to events when enabled
    private void OnEnable()
    {
        GameManager.OnChangedState += ChangeGameState; //Subscribe the method ChangeGameState to the OnChangedState event
    }

    //Subscribe to events when disabled
    private void OnDisable()
    {
        GameManager.OnChangedState -= ChangeGameState; //Unsubscribe the method ChangeGameState to the OnChangedState event
    }

    //Make something based on the current game state
    public void ChangeGameState(GameManager.GameState gameState)
    {
        //Check the current game state
        switch (gameState)
        {
            case GameManager.GameState.RUNNING:

                pauseButton.interactable = true; //Make the pause button interactable

                break;

            case GameManager.GameState.GAMEOVER:

                distanceGameOverScore.text = (GameManager.Instance.CurrentDistance.ToString() + "m"); //Call the UIManager to change the game over distance score text
                bestDistanceGameOverScore.text = (PlayerPrefs.GetFloat("BestDistance").ToString() + "m"); //Call the UIManager to change the game over best distance score text
                bloodGameOverScore.text = GameManager.Instance.BloodTaken.ToString(); //Call the UIManager to change the game over blood score text
                BestBloodScoreInOneRun.text = PlayerPrefs.GetInt("BestBlood").ToString(); //Call the UIManager to change the game over best blood score text
                pauseButton.interactable = false; //Make the pause button not interactable

                break;

            case GameManager.GameState.PAUSED:

                distacePausedScore.text = (GameManager.Instance.CurrentDistance.ToString() + "m"); //Call the UIManager to change the pause distance score text
                bloodPausedScore.text = GameManager.Instance.BloodTaken.ToString(); //Call the UIManager to change the pause blood score text
                pauseButton.interactable = false; //Make the pause button not interactable

                break;
        }
    }

    //Pause the game
    public void PauseClick()
    {
        UIManager.Instance.AnimatePanels(pauseOptionsPanel, Vector3.zero, .5f, true); //Call the UIManager to animate the pause panel left side
        UIManager.Instance.AnimatePanels(pauseSettingsPanel, Vector3.zero, .5f, true); //Call the UIManager to animate the pause panel right side
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
        GameManager.Instance.PauseGame(); //Call the Game manager method to pause the game
    }

    //Continue the game after it being paused
    public void ContinueGame()
    {
        UIManager.Instance.SaveConfigs(backgroundVolume.value, sfxVolume.value); //Call the UI Manager method to save the configurations
        UIManager.Instance.AnimatePanels(pauseOptionsPanel, pauseOptionsInitialPos, .5f, false); //Call the UIManager to animate the pause panel left side
        UIManager.Instance.AnimatePanels(pauseSettingsPanel, pauseSettingsInitialPos, .5f, false); // Call the UIManager to animate the pause panel right side
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
        GameManager.Instance.ContinueGamePaused(); //Call the Gama Manager method to continue the game 
    }

    //Method that will be called when the value of the volume slider is modified
    public void AdjustVolumePauseMenu(bool backGround)
    {
        if (backGround)
        {
            AudioManager.Instance.AdjustVolume(backgroundVolume.value, true); //Call the Audio manager method to change the volume of determined audio sources
        }
        else
        {
            AudioManager.Instance.AdjustVolume(sfxVolume.value, false); //Call the Audio manager method to change the volume of determined audio sources
        }
    }

    //Go back to the main menu
    public void GoToMainMenu()
    {
        GameManager.Instance.ResetGame(); //Call the game manager to reset thegame variables
        AudioManager.Instance.RainClip = null; //Stop the rain sound effect
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
        LoadingManager.Instance.Unloadlevel("Game"); //Call the Loading Manager method to unload the game scene
        LoadingManager.Instance.LoadLevel("MainMenu"); //Call the Loading Manager method to load the main menu scene
        
    }

    //Open page to rate the game
    public void RateGameGameOver()
    {
        UIManager.Instance.RateGame(); //Call the UI Manager method to rate the game
    }

    //Show the game over menu
    public void ShowGameOverMenu()
    {
        //Check if there is already a death count variale
        if (!PlayerPrefs.HasKey("DeathCount"))
        {
            PlayerPrefs.SetInt("DeathCount", 0); //Set the death count variable
        }

        //Check if the player died more than 3 times
        if (PlayerPrefs.GetInt("DeathCount") >= 3)
        {
            ShowAD("video"); //Call the method to play an AD
            PlayerPrefs.SetInt("DeathCount", 0); //Reset the death count
        }
        else
        {
            int deathCount = PlayerPrefs.GetInt("DeathCount") + 1; //Increase the death count
            PlayerPrefs.SetInt("DeathCount", deathCount); //Set the new death count
        }

        UIManager.Instance.AnimatePanels(gameoverScore, Vector3.zero, .5f, true); //Call the UIManager to animate the game over top panel side
        UIManager.Instance.AnimatePanels(gameoverOptions, Vector3.zero, .5f, true); //Call the UIManager to animate the game over bottom panel side
    }

    public void ShowContinue()
    {
        UIManager.Instance.AnimatePanels(continuePanel, Vector3.zero, .5f, true); //Call the UIManager to animate the continue top panel side
        UIManager.Instance.AnimatePanels(endRunPanel, Vector3.zero, .5f, true); //Call the UIManager to animate the continue bottom panel side

        //Check if the player has the necessary amout of blood
        if (PlayerPrefs.GetInt("BloodCount") >= necessaryBlood)
        {
            continueBloodButton.interactable = true; //Make the continue paying blood button interactable
        }
        else
        {
            continueBloodButton.interactable = false; //Make the continue paying blood button not interactable
        }
        coroutine = StartCoroutine(Timer()); //Start the timer coroutine
    }

    private void EndContinue()
    {
        UIManager.Instance.AnimatePanels(continuePanel, continuelPanelInitialPos, .5f, false); //Call the UIManager to animate the continue top panel side
        UIManager.Instance.AnimatePanels(endRunPanel, EndRunInitialPos, .5f, false); //Call the UIManager to animate the continue bottom panel side
    }

    public void EndRun()
    {
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
        StopCoroutine(coroutine); //Stop the timer coroutine
        LeanTween.move(continuePanel, continuelPanelInitialPos, .5f).setIgnoreTimeScale(true); //Animate the continue top panel side
        LeanTween.move(endRunPanel, EndRunInitialPos, .5f).setIgnoreTimeScale(true).setOnComplete(ShowGameOverMenu); //Animate the continue top panel side
    }

    //Reload the scene
    public void TryAgain()
    {
        GameManager.Instance.ResetGame(); //Call the game manager to reset thegame variables
        AudioManager.Instance.RainClip = null; //Stop the rain sound effect
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
        LoadingManager.Instance.ReloadLevel("Game"); //Call the loading manager to reaload the game scene
    }

    //Continue the game after the player has lost
    public void ContinueGameAfterLose(bool ad)
    {
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
        if (ad)
        {
            ShowAD("rewardedVideo"); //Show an AD
            while (Advertisement.isShowing)
            {
                return;
            }
            EndContinueGameAfterLoseFunction();
        }
        else
        {
            PlayerPrefs.SetInt("BloodCount", (PlayerPrefs.GetInt("BloodCount") - necessaryBlood)); //Decrease the blood count
            UpdateBlood(); //Call the method to update the blood count UI
            EndContinueGameAfterLoseFunction();
        }
    }

    private void EndContinueGameAfterLoseFunction()
    {
        EndContinue(); //Call the method to hide the continue panel
        StopCoroutine(coroutine); //Stop the timer coroutine
        GameManager.Instance.ContinueGameAfterLose(); //Call the Game Manager method to continue the game 
    }

    //Coroutine to the continue run timer
    private IEnumerator Timer()
    {
        
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(1); //Delay to continue
        int timer = timerEndRun; //Set the timer

        //Loop to do the timer
        while (timer > 0)
        {
            UIManager.Instance.UpdateText(continueTimer, timer.ToString()); //Call the UIManager to update the timer UI display
            timer--; //Decease the timer
            yield return delay; //Wait the delay
        }
        EndRun(); //Hide the continue panel
    }

    //Show an ad
    private void ShowAD(string type)
    {
        //Check if the AD is ready
        if (Advertisement.IsReady())
        {
            Advertisement.Show(type); //Fire the AD
        }
    }

    //Reset the player HPs UI
    public void ResetLife()
    {
        //Loop to recover all the HPs
        for (int i = 0; i < hpAnimator.Count; i++)
        {
            UIManager.Instance.HPAnimation(hpAnimator[i], false); //Call the UIManager to show the HP display
        }
    }

    //Update the blood count
    public void UpdateBlood()
    {
        UIManager.Instance.UpdateText(bloodCount, PlayerPrefs.GetInt("BloodCount").ToString()); //Call the UI Manager to update the UI blood score
    }

    //Update the initial Timer
    public void UpdateInitialTimer(int time)
    {
        //Check if the timer is negative 0
        if (time >= 0)
        {
            initialTimer.gameObject.SetActive(true); //Enable the initial timer UI
            UIManager.Instance.UpdateText(initialTimer, time.ToString()); //Call the UIManager to update the UI initial timer
        }
        else
        {
            UIManager.Instance.EnableDisableUI(initialTimer.gameObject); //Call the UIManager to disable the initial timer
        }
    }

    //Update the distance score
    public void UpdateDistance()
    {
        UIManager.Instance.UpdateText(distanceScore, (GameManager.Instance.CurrentDistance.ToString() + "m")); //Call the UIManager to update the UI distance score
    }

    //Update the HP UI
    public void UpdateHP(bool lose)
    {
        //Check if the player is losing or recovering a HP
        if (lose)
        {
            UIManager.Instance.HPAnimation(hpAnimator[GameManager.Instance.CurrentHP], lose); //Call the UI Manager to animate losing a HP UI
        }
        else
        {
            UIManager.Instance.HPAnimation(hpAnimator[GameManager.Instance.CurrentHP - 1], lose); //Call the UI Manager to animate recovering a HP UI
        }
    }
}

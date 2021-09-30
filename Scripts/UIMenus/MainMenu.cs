using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Slider backgroundVolume; //Reference to the slider that provide the background volume value
    [SerializeField] private Slider sfxVolume; //Reference to the slider that provide the sound effect volume value
    [SerializeField] private RectTransform settingsPanel; //Reference to the settings panel
    [SerializeField] private RectTransform creditsPanel; //Reference to the credits panel
    [SerializeField] private Text playGameText;
    [SerializeField] private Text bloodCount;
    [SerializeField] private GameObject canvas;
    [SerializeField] private AudioClip clickSFX; //Audio clip that will be played when the a button is clicked

    private Vector3 settingsPanelInitialPosition; //Settings Panel Initial Position
    private Vector3 creditsPanelInitialPosition; //Credits Panel Initial Position

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("PlayedTutorial"))
        {
            PlayerPrefs.SetInt("PlayedTutorial", 0);
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 1;
        InvokeRepeating("AnimateText", 0f, .8f);
        bloodCount.text = PlayerPrefs.GetInt("BloodCount").ToString();
        LoadingManager.OnFadedOut += EnableCanvas;
    }

    private void OnDisable()
    {
        LoadingManager.OnFadedOut -= EnableCanvas;
    }

    //Set the panels initial positions at the start of the scene
    private void Start()
    {
        settingsPanelInitialPosition = settingsPanel.anchoredPosition; //Set the settings Panel Initial Position
        creditsPanelInitialPosition = creditsPanel.anchoredPosition; //Set the credits Panel Initial Position
        backgroundVolume.value = AudioManager.Instance.BackgroundVolume; //Set the volume to the background slider
        sfxVolume.value = AudioManager.Instance.SfxVolume; //Set the volume to the sound effects slider
        
    }

    private void AnimateText()
    {
        if (playGameText.color.a <= 0)
        {
            LeanTween.alphaText(playGameText.rectTransform, 1f, .7f);
        }
        else
        {
            LeanTween.alphaText(playGameText.rectTransform, 0f, .7f);
        }
    }

    private void EnableDisableSettings()
    {
        settingsPanel.gameObject.SetActive(!settingsPanel.gameObject.activeSelf);
    }

    private void EnableDisableCredits()
    {
        creditsPanel.gameObject.SetActive(!creditsPanel.gameObject.activeSelf);
    }

    //Method that will be called when the button options is pressed
    public void ShowOptionsMenu()
    {
        EnableDisableSettings();
        LeanTween.move(settingsPanel, Vector3.zero, .5f); //Make Animation
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
    }

    //Method that will be called when the button save configurations is pressed
    public void SaveConfigsMainMenu()
    {
        UIManager.Instance.SaveConfigs(backgroundVolume.value,sfxVolume.value); //Call the UI Manager method to save the configurations
        LeanTween.move(settingsPanel, settingsPanelInitialPosition, .5f).setOnComplete(EnableDisableSettings);//Make Animation
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
    }

    //Method that will be called when the button credits is pressed
    public void ShowCredits()
    {
        EnableDisableCredits();
        LeanTween.move(creditsPanel, Vector3.zero, .5f);//Make Animation
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
    }

    //Method that will be called when the button close credits is pressed
    public void HideCredits()
    {
        LeanTween.move(creditsPanel, creditsPanelInitialPosition, .5f).setOnComplete(EnableDisableCredits);//Make Animation
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
    }

    //Method that will be called when the button play is pressed
    public void GoToGame()
    {
        if (PlayerPrefs.GetInt("PlayedTutorial") > 0)
        {
            AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
            LoadingManager.Instance.Unloadlevel("MainMenu"); //Call the Loading Manager method to unload the main menu scene
            LoadingManager.Instance.LoadLevel("Game"); //Call the Loading Manager method to load the game scene
        }
        else
        {
            GoToTutorial();
        }
        
    }

    //Method that will be called when the button tutorial is pressed
    public void GoToTutorial()
    {
        if (PlayerPrefs.GetInt("PlayedTutorial") <= 0)
        {
            PlayerPrefs.SetInt("PlayedTutorial", 1);
        }
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
        LoadingManager.Instance.Unloadlevel("MainMenu"); //Call the Loading Manager method to unload the main menu scene
        LoadingManager.Instance.LoadLevel("Tutorial"); //Call the Loading Manager method to load the tutorial scene
    }

    //Method that will be called when the value of the volume slider is modified
    public void AdjustVolumeMainMenu(bool backGround)
    {
        //Check if the volume is from the background or the sound effects
        if (backGround)
        {
            AudioManager.Instance.AdjustVolume(backgroundVolume.value, true); //Call the Audio manager method to change the volume of determined audio sources
        }
        else
        {
            AudioManager.Instance.AdjustVolume(sfxVolume.value, false); //Call the Audio manager method to change the volume of determined audio sources
        }
    }

    //Method that will be called when the button rate game is pressed
    public void RateGameMainMenu()
    {
        AudioManager.Instance.PlaySFX(0, clickSFX); //Call the Audio Manager to play the click audio clip;
        UIManager.Instance.RateGame(); //Call the UI Manager method to open the page of the game on the PlayStore or AppleStore
    }

    public void EnableCanvas()
    {
        canvas.SetActive(true);
    }
}

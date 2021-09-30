using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource backgroundAudio; //Background audio source
    [SerializeField] private AudioSource uiAudio; //UISFX audio source
    [SerializeField] private AudioSource jumpAudio; //JumpSFX audio source
    [SerializeField] private AudioSource hitAudio; //HitSFX audio source
    [SerializeField] private AudioSource rainAudio; //RainSFX audio source
    [SerializeField] private AudioClip mainMenuMusic; //Background music for the main menu
    [SerializeField] private AudioClip tutorialMusic; //Background music for the tutorial

    //Property to get the background volume value
    public float BackgroundVolume
    {
        get { return backgroundAudio.volume; } //Return the background volume
    }

    //Property to get the sound effects volume value
    public float SfxVolume
    {
        get { return uiAudio.volume; } //Return the sound effects volume
    }

    //Property to set the rain sound effect
    public AudioClip RainClip
    {
        set { rainAudio.clip = value; } //Set the rain sound effect
    }

    //Don't destroy the object when a scene is loaded and check if there is audio settings saved to be loaded
    private void Start()
    {
        DontDestroyOnLoad(gameObject); //Don't destroy this object when the scene is loaded

        if (PlayerPrefs.HasKey("CurrentVolumeBackground") && PlayerPrefs.HasKey("CurrentVolumeSFX")) //Check if there is volume settings saved
        {
            AdjustVolume(PlayerPrefs.GetFloat("CurrentVolumeBackground"), true); //Adjust the volume of the background to the volume previously saved
            AdjustVolume(PlayerPrefs.GetFloat("CurrentVolumeSFX"), false); //Adjust the volume of the sound effects to the volume previously saved
        }
    }

    //Subscribe to events when enabled
    private void OnEnable()
    {
        LoadingManager.OnLoadComplete += ChangeBackgrounMusicOnLoading; //Subscribe the method ChangeBackgrounMusicOnLoading to the event OnLoadComplete
    }

    //Unsubscribe from the events when disabled
    private void OnDisable()
    {
        LoadingManager.OnLoadComplete -= ChangeBackgrounMusicOnLoading; //Unsubscribe the method ChangeBackgrounMusicOnLoading to the event OnLoadComplete
    }

    //Method to be called when scene load and unload is completed
    private void ChangeBackgrounMusicOnLoading(string levelName)
    {
        //Check which scene has been loaded
        switch (levelName)
        {
            //Call method to change the background music passing the main menu music
            case "MainMenu":
                ChangeBackGroundMusic(mainMenuMusic, 1);
                break;

            //Call method to change the background music passing the tutorial music
            case "Tutorial":
                ChangeBackGroundMusic(tutorialMusic, 1);
                break;

            //Call method to change the background music passing the game scene music
            case "Game":
                backgroundAudio.Stop();
                break;

            default:
                break;
        }
    }

    //Change the background music and play it
    public void ChangeBackGroundMusic(AudioClip music, float pitchValue)
    {
        backgroundAudio.clip = music; //Assing the new music to the background audio source
        backgroundAudio.pitch = pitchValue; //Adjust the pitch of the music
        backgroundAudio.Play(); //Play the music
    }

    //Play a sound effect
    public void PlaySFX(int typeOfSFX, AudioClip sfx)
    {
        //Check whitch type of sound effect needs to be played
        switch (typeOfSFX)
        {
            //Play a UI sound effect
            case 0: // UI SFX
                uiAudio.PlayOneShot(sfx);
                break;

            //Play a jump sound effect
            case 1: // Jump SFX
                jumpAudio.PlayOneShot(sfx);
                break;

            //Play a hit sound effect
            case 2: // Hit SFX
                hitAudio.PlayOneShot(sfx);
                break;

            //Play the rain sound effect
            case 3: // rain SFX
                rainAudio.clip = sfx; //Set the rain sound effect
                rainAudio.Play(); //Play the sound effect
                break;

            default:
                break;
        }
    }

    //Adjust the volume of the audio source
    public void AdjustVolume(float volume, bool background)
    {
        //Check if the volume being adjusted is from the background or the SFX
        if (background)
        {
            backgroundAudio.volume = volume; //Adjust the background volume
        }
        else
        {
            uiAudio.volume = volume; //Adjust the UI sound effect volume
            jumpAudio.volume = volume; //Adjust the Jump sound effect volume
            hitAudio.volume = volume; //Adjust the hit sound effect volume
            rainAudio.volume = (volume -.3f); //Adjust the rain sound effect volume
        }
    }

    //Mute a audio source
    public void Mute(bool background)
    {
        //Check if should mute the background music or the sound effects
        if (background)
        {
            backgroundAudio.mute = !backgroundAudio.mute; //Mute the background music
        }
        else
        {
            uiAudio.mute = !uiAudio.mute; //Mute the UI sound effects
            jumpAudio.mute = !jumpAudio.mute; //Mute the jump sound effects
            hitAudio.mute = !hitAudio.mute; //Mute the hit sound effects
            rainAudio.mute = !rainAudio.mute; //Mute the rain sound effects
        }
    }
}

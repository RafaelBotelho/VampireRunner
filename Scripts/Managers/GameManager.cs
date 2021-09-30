using SleekRender;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public delegate void ChangeGameState(GameState gameState); //Delegate for when the game state is changed
    public delegate void ResetGameScene(); //Delegate for when the game scene is reseted

    public static event ChangeGameState OnChangedState; //Event for when the game state is changed
    public static event ResetGameScene OnReset; //Delegate for when the game scene is reseted

    [SerializeField]private float currentDistance; //Current score
    public float CurrentDistance //Current score property
    {
        get { return currentDistance; } //Return the current score
    }

    [SerializeField] private int bloodTaken; //Current score
    public int BloodTaken //Current score property
    {
        get { return bloodTaken; } //Return the current score
    }

    [SerializeField]private int currentHP; //Current life
    public int CurrentHP //Current life property
    {
        get { return currentHP; } //Return the current life
    }

    private int initialTimer = 3; //Initial timer
    public int InitialTimer //Initial timer property
    {
        get { return initialTimer; } //Return the initial timer value
    }

    [SerializeField] private int maxHP; //Max life possible
    public int MaxHP //Max life Property
    {
        get { return maxHP; } //Return the max life possible
    }

    [SerializeField] private AudioClip timerClip; //Reference to the initial timer audio clip
    [SerializeField] private AudioClip gameOverClip; //Reference to the game over audio clip
    [SerializeField] private AudioClip rainClip; //Reference to the rain audio clip
    [SerializeField] private List<AudioClip> gameRunningClips; //Reference to the game running audio clip
    [SerializeField] private AudioClip gameRunningDyingClip;
    [SerializeField] private float valueToIncreaseSpeed; // Value that will be increased to the speed
    [SerializeField] private float secondsToWaitToIncreaseSpeed; //Seconds To Wait To Increase Speed of the objects
    [SerializeField] private float speedLimit; //Max speed the objects can have
    [SerializeField] private SleekRenderSettings settings; //Post processing settings
    [SerializeField] private List<GameObject> skys; //List of skys avaliable
    [SerializeField] private GameObject rain; //Reference to the rain gameobject
    [SerializeField] private GameUIController gameUIController; //Reference to the Game Scene UI Controller
    private bool started = false; //If the coroutines have started
    private bool startedMusic = false; //If the music has already started

    //Enum that contain the game states
    public enum GameState
    {
        PREGAME, RUNNING, PAUSED, GAMEOVER //Game states
    }

    private GameState currentGameState; //Current game state

    //Current game state property
    public GameState CurrentGameState
    {
        get { return currentGameState; } //Return the current game state
        private set { currentGameState = value; } //Set a new game state
    }

    //Update the current game state
    private void UpdateState(GameState state)
    {
        currentGameState = state; //Update the current game state

        //Check with one is the current game state
        switch (currentGameState)
        {
            case GameState.PREGAME:

                ResetGame(); //Reset the game settings

                //Check if already exist a best distance score saved
                if (!PlayerPrefs.HasKey("BestDistance"))
                {
                    PlayerPrefs.SetFloat("BestDistance", 0); //Create a best distance score save
                }

                //Check if already exist a best blood score saved
                if (!PlayerPrefs.HasKey("BestBlood"))
                {
                    PlayerPrefs.SetInt("BestBlood", 0); //Create a best blood score save
                }
                StartCoroutine(StartGame()); //Start the coroutine to start the game

                break;
            case GameState.RUNNING:

                //Check if the coroutines have already started
                if (!started)
                {
                    StartCoroutine(IncreasesSpeed()); //Start the coroutine to increase the speed of the objects
                }

                //Check if the music has already started
                if (!startedMusic)
                {
                    startedMusic = true; //Set that the music has started
                    AudioManager.Instance.ChangeBackGroundMusic(gameRunningClips[Random.Range(0, gameRunningClips.Count)], 1); //Start the background music
                }
                Time.timeScale = 1; //Set the time scale to 1

                break;
            case GameState.PAUSED:

                Time.timeScale = 0; //Set the time scale to 0

                break;
            case GameState.GAMEOVER:

                Time.timeScale = 0; //Set the time scale to 0
                AudioManager.Instance.ChangeBackGroundMusic(gameOverClip, 1); //Play the game over music
                startedMusic = false; //Set that the music has stopped

                //Check if the current distance score is greater than the best distance score previously saved
                if (PlayerPrefs.GetFloat("BestDistance") < currentDistance)
                {
                    PlayerPrefs.SetFloat("BestDistance", currentDistance); //Set a new best distance score
                }

                //Check if the current blood score is greater than the best blood score previously saved
                if (PlayerPrefs.GetInt("BestBlood") < bloodTaken)
                {
                    PlayerPrefs.SetInt("BestBlood", bloodTaken); //Set the new best blood score
                }

                break;
            default:
                break;
        }

        //Check if there are listeners to the OnChangedState event
        if (OnChangedState != null)
        {
            OnChangedState(CurrentGameState); //Fire the OnChangedState event
        }
    }

    //Subscribe to events when enabled
    private void OnEnable()
    {
        PlayerController.OnHitted += UpdateHit; //Subscribe the UpdateHit method to the OnHitted event
    }

    //Unsubscribe to events when disabled
    private void OnDisable()
    {
        PlayerController.OnHitted -= UpdateHit; //Unsubscribe the UpdateHit method to the OnHitted event
    }

    //Set the background sky and if it is raining
    private void Start()
    {
        //Deactivate all skys
        foreach (GameObject sky in skys)
        {
            sky.SetActive(false); //Deativate the sky
        }

        int skyIndex = Random.Range(0, skys.Count); //Slect a random sky

        skys[skyIndex].SetActive(true); //Activate a random sky

        //Check if it is a clouded sky
        if (skyIndex != 0)
        {
            int activateRain = Random.Range(0, 2); //Select a random number to decide if it will rain or not

            //Check if it will be raining
            if (activateRain == 1)
            {
                rain.SetActive(true); //Activate the rain
                AudioManager.Instance.PlaySFX(3, rainClip); //Play the rain sound effect
            }
        }

        Time.timeScale = 0; //Set time scale to 0
    }

    //Start the pre game
    public void PreGame()
    {
        UpdateState(GameState.PREGAME); //Update the game state to pre game
    }

    //Coroutine that will start the game
    private IEnumerator StartGame()
    {
        initialTimer = 3; //Set the initial timer

        //Loop for a number of seconds
        while (initialTimer >= 0)
        {
            initialTimer--; //Decrement the initial timer
            gameUIController.UpdateInitialTimer(initialTimer); //Update the initial timer UI value
            AudioManager.Instance.PlaySFX(0, timerClip); //Play the timer audio clip
            yield return new WaitForSecondsRealtime(.7f); //Wait for .7 second
        }
        UpdateState(GameState.RUNNING); //Update the game state
    }

    //Pause the game
    public void PauseGame()
    {
        UpdateState(GameState.PAUSED); //Update the game state
    }

    //Continue the game after it been paused
    public void ContinueGamePaused()
    {
        StartCoroutine(StartGame()); //Start the coroutine to start the game
    }

    //Continue the game after the player lose all their lives
    public void ContinueGameAfterLose()
    {
        currentHP = maxHP; //Resets the player HP
        gameUIController.ResetLife(); //Call the GameUIController Manager to update the player HPs UI
        StartCoroutine(StartGame()); //Start the coroutine to start the game
    }

    //Coroutine that will increment the objects speed
    private IEnumerator IncreasesSpeed()
    {
        WaitForSeconds delay = new WaitForSeconds(secondsToWaitToIncreaseSpeed); //Delay to continue
        started = true; //Set that the coroutine have started

        //Loop that will increment the objects speed while the game is running
        while (CurrentGameState == GameState.RUNNING) //Update the game state
        {
            //Check if the current speed is not at the limit
            if (MoveLeft.increaseSpeed <= speedLimit)
            {
                MoveLeft.increaseSpeed += valueToIncreaseSpeed; //Increase the objects speed
            }
            currentDistance++; //Increment the current distance
            gameUIController.UpdateDistance(); //Update the current distance UI
            yield return delay; //Wait the delay
        }
        started = false; //Set that the coroutine have stoped
    }

    //Update the player score and restore a HP
    private void UpdateHit(int blood, int hp, Color vignetteColor)
    {
        bloodTaken += blood; //Set the new current blood value
        int tempBlood = PlayerPrefs.GetInt("BloodCount");
        tempBlood += blood;
        PlayerPrefs.SetInt("BloodCount", tempBlood); //Increment the player blood count
        gameUIController.UpdateBlood(); //Call the GameUIController to update the UI score

        //Switch based on the HP to be updated
        switch (hp)
        {
            //Case the player lost a HP
            case -1:

                currentHP--; //Decrease the HP
                gameUIController.UpdateHP(true); //Call the GameUIController to lose 1 HP in the UI display
                MoveLeft.increaseSpeed--; //Increase the objects speed

                //Check if the increase speed variable is < 0
                if (MoveLeft.increaseSpeed < 0)
                {
                    MoveLeft.increaseSpeed = 0; //Set the variable to 0
                }

                //Check if the current HP is 1 or less
                if (currentHP <= 1)
                {
                    AudioManager.Instance.ChangeBackGroundMusic(gameRunningDyingClip, 1.5f); //Play the dying clip
                }

                break;

                //Do nothing
            case 0:
                break;

            //Case the player recovered a HP
            case 1:

                //Check if the player isn't already at max HP
                if (currentHP < maxHP)
                {
                    currentHP++; //Increase the HP
                    gameUIController.UpdateHP(false); //Call the GameUIController to recover 1 HP in the UI display

                    //Check if the player has more than 1 HP
                    if (currentHP == 2)
                    {
                        AudioManager.Instance.ChangeBackGroundMusic(gameRunningClips[Random.Range(0, gameRunningClips.Count)], 1); //Play a random background music
                    }
                }

                break;

            default:
                break;
        }

        //Check if the player doesn't have anymore HP 
        if (currentHP <= 0)
        {
            UpdateState(GameState.GAMEOVER); //Update the game state
        }
    }

    //Reset the game settings
    public void ResetGame()
    {
        currentHP = maxHP; //Reset the player HP
        currentDistance = 0; //Reset the score
        bloodTaken = 0; //Reset the blood taken
        MoveLeft.increaseSpeed = 0; //Reset the speed of all objects
        settings.vignetteEnabled = false; //Disable the vignette post processing

        //Check if it isn't the pre game
        if (CurrentGameState != GameState.PREGAME)
        {
            //Fire the onreset event
            if (OnReset != null)
            {
                OnReset();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class LoadingManager : Singleton<LoadingManager>
{
    public delegate void LoadScene(string sceneName); //Delegate for when a scene is loaded
    public delegate void UnloadScene(); //Delegate for when a scene is unloaded
    public delegate void FadeOut(); //Delegate for when the fade out has been complete

    public static event LoadScene OnLoadComplete; //Event for when a scene is loaded
    public static event UnloadScene OnUnloadComplete; //Event for when a scene is unloaded
    public static event FadeOut OnFadedOut; //Event for when the fade out has been complete

    private List<AsyncOperation> loadOperations; //List of scenes being loaded
    private string currentLevelName = string.Empty; //Name of the current scene loaded
    private bool reload; //If the scene will be reload
    private string reloadName; //The name of the scene that need to be reloaded

    [SerializeField] private RectTransform loadingScreenBackground; //Loading screen panel
    [SerializeField] private RectTransform loadingScreenIcon1;  //Loading screen loading icon 1
    [SerializeField] private RectTransform loadingScreenIcon2;  //Loading screen loading icon 1
    [SerializeField] private GameObject loadingScreenCanvas;  //Loading screen canvas
    [SerializeField] private GameObject loadingScreenEventSystem;  //Loading screen event system
    [SerializeField] private GameObject loadingMainCamera; //Loading scene main camera

    private bool testMode = true;

    //Property to get the current scene name
    public string CurrentLevelName
    {
        get { return currentLevelName; }
    }

    //Start the application
    private void Start()
    {
        Application.targetFrameRate = 60;

        Advertisement.Initialize("4061491", testMode);
        DontDestroyOnLoad(gameObject); //Don't destroy this object when the scene is unloaded
        loadOperations = new List<AsyncOperation>(); //initialize list of scenes to be loaded
        LoadLevel("MainMenu"); //Load Main Menu scene
    }

    //Load a scene
    public void LoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive); //Load a scene and store the operation on a local variable

        //Check if operation is null
        if (ao == null)
        {
            Debug.LogError("[LoadingManager] Unable to load level " + levelName); //Debug a error
            return;
        }

        loadOperations.Add(ao); //Add new operation to the list of operations being executed
        ao.completed += LoadOperationComplete; //When operation is completed fire event calling method LoadOperationComplete
        currentLevelName = levelName; //Store the current level name
    }

    //Unload a scene
    public void Unloadlevel(string levelName)
    {
        StartCoroutine(FadeInOut(true)); //Call function to fade in the loading screen

        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName); //Unload a scene and store the operation on a local variable

        //Check if operation is null
        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to unload level " + levelName); //Debug a error
            return;
        }

        currentLevelName = "LoadingScreen"; //Store the current level name
        ao.completed += UnloadOperationComplete; //When operation is completed fire event calling method UnloadOperationComplete
    }

    //Method to be called when scene load is completed
    private void LoadOperationComplete(AsyncOperation ao)
    {
        //Remove the scene loaded from the list of operations
        if (loadOperations.Contains(ao))
        {
            loadOperations.Remove(ao);
        }

        StartCoroutine(FadeInOut(false)); //Call function to fade out the loading screen

        //fire event on load complete
        if (OnLoadComplete != null)
        {
            OnLoadComplete(currentLevelName);
        }
    }

    //Method to be called when scene unload is completed
    private void UnloadOperationComplete(AsyncOperation ao)
    {
        //Check if the scene being unloaded will be reloaded
        if (reload)
        {
            reload = false; //Reset the reload variable controller
            LoadLevel(reloadName); //Reaload the scene
            reloadName = ""; //Reset the reload variable controller name
        }
        //Fire event on unload complete
        if (OnUnloadComplete != null)
        {
            OnUnloadComplete();
        }
    }

    //Reload a scene
    public void ReloadLevel(string levelName)
    {
        reload = true; //Set tahat the scene need to be reloaded
        reloadName = levelName; //Set the name of the scene that need to be reloaded
        Unloadlevel(levelName); //Unload the scene so it can be reloaded
    }

    //Fade in and out the loading screen
    private IEnumerator FadeInOut(bool FadeIn)
    {
        //Check if should fade in or fade out
        if (FadeIn)
        {
            EnableLoadingScreen(); //Call the the method to enable the loading screen
            LeanTween.alpha(loadingScreenBackground, 1, 0).setIgnoreTimeScale(true); //Animate the  loading screen to fade in
            LeanTween.alpha(loadingScreenIcon1, 1, 0).setIgnoreTimeScale(true); //Animate the  loading screen to fade in
            LeanTween.alpha(loadingScreenIcon2, 1, 0).setIgnoreTimeScale(true); //Animate the  loading screen to fade in
        }
        else
        {
            LeanTween.alpha(loadingScreenBackground, 0, .5f).setIgnoreTimeScale(true); //Animate the  loading screen to fade out
            LeanTween.alpha(loadingScreenIcon2, 0, .5f).setIgnoreTimeScale(true); //Animate the  loading screen to fade out
            LeanTween.alpha(loadingScreenIcon1, 0, .5f).setIgnoreTimeScale(true).setOnComplete(DisableLoadingScreen); //Animate  loading screen to fade out and call the the method to disable the loading screen
        }

        yield return new WaitForSeconds(1); //Wait for 1 second
    }

    //Disable the Loading screen objects
    private void DisableLoadingScreen()
    {
        loadingMainCamera.SetActive(false); //Disable the loading scene camera
        loadingScreenCanvas.SetActive(false); //Disable the loading scene background
        loadingScreenEventSystem.SetActive(false); //Disable the loading scene event scene

        //Check if the current scene is the game scene
        if (currentLevelName == "Game")
        {
            GameManager.Instance.PreGame(); //Call the game manager to start the pre game
        }

        //Fire the event OnFadedOut
        if (OnFadedOut != null)
        {
            OnFadedOut();
        }
    }

    //Enable the Loading screen objects
    private void EnableLoadingScreen()
    {
        loadingMainCamera.SetActive(true); //Enable the loading scene camera
        loadingScreenCanvas.SetActive(true); //Enable the loading scene background
        loadingScreenEventSystem.SetActive(true); //Enable the loading scene event scene
    }
}

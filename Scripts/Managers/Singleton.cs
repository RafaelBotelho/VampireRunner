using UnityEngine;

//Managers Base class
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance; //Class instance
    public static T Instance //Class instance property
    {
        get { return instance; } //Return the class instance
    }

    //Return if the singleton is initialized property
    public static bool IsInitialized
    {
        get { return instance != null; } //Return if the singleton is initialized
    }

    //Populate the instance
    protected virtual void Awake()
    {

        //Check if already exists an instance of this class
        if (instance != null)
        {
            Debug.LogError("[Singleton] Trying to instantiate a second instance of a singleton class."); //Debug a error
        }
        else
        {
            instance = (T)this; //Populate the instance
        }
    }

    //Clear the instance when object is destroyed
    protected virtual void OnDestroy()
    {
        //Check if this is an instance
        if (instance == this)
        {
            instance = null; //Clear the instance
        }
    }
}

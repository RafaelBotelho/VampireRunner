using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefabObject; //Prefab that will fill the pool
    [SerializeField] private int poolDepth; //Size of the pool
    [SerializeField] private bool canGrow = true; //Variable that specify if the pool can grow during gameplay

    private List<GameObject> pool = new List<GameObject>(); //List that will be used as the pool

    //Instantiate the objects and store them in the pool
    private void Awake()
    {
        //Loop to populate the pool 
        for (int i = 0; i < poolDepth; i++)
        {
            GameObject pooledObject = Instantiate(prefabObject); //Instantiate the prefab and store it in a local variable
            pooledObject.transform.parent = gameObject.transform;
            pooledObject.SetActive(false); //Deactivate the object
            pool.Add(pooledObject); //Store the object in the pool
        }
    }

    //Subscribe to the OnReset Event
    private void OnEnable()
    {
        GameManager.OnReset += ResetPool;
    }

    //Unsubscribe to the OnReset Event
    private void OnDisable()
    {
        GameManager.OnReset -= ResetPool;
    }

    //Return a avaliable object to be used or instantiate a new one to be used if possible
    public GameObject GetAvailableObject()
    {
        //Loop that will run through the list looking for a avaliable object
        for (int i = 0; i < pool.Count; i++)
        {
            //Check if the object is avaliable
            if (pool[i].activeInHierarchy == false)
            {
                return pool[i]; //Return the object avaliable
            }
        }

        //Check if the pool can grow
        if (canGrow == true)
        {
            GameObject pooledObject = Instantiate(prefabObject); //Instantiate the prefab and store it in a local variable
            pooledObject.transform.parent = gameObject.transform;
            pooledObject.SetActive(false); //Deactivate the object
            pool.Add(pooledObject); //Store the object in the pool

            return pooledObject; //Return the new object avaliable
        }
        else
        {
            return null; //Return no avaliable objects
        }
    }

    //Reset the pool
    private void ResetPool()
    {
        pool.Clear(); //Clear the pool
    }
}

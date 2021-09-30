using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour
{

    [SerializeField] private Vector2 spawnPoint; //House spawn point information
    private bool visible; //If the object has already beacame visible

    //House spawn point property
    public Vector2 SpawnPoint 
    {
        get { return spawnPoint; } //Get the house spawn point stored
    }
    
    private List<Vector2> previousSortedPoint = new List<Vector2>(); //List of previous character spawn points used 
    private Vector2 sortedSpawnPoint = new Vector2(); //Current character spawn point sorted
    public List<GameObject> PeopleSpawnPoints; //List of spawn points avaliable

    //Get a character spawn point avaliable
    public Vector2 GetPeopleSpawnPoint()
    {
        sortedSpawnPoint = PeopleSpawnPoints[Random.Range(0, PeopleSpawnPoints.Count)].transform.position; //Store the spawn point sorted

        //Loop to get a spawn point avaliable
        while (previousSortedPoint.Contains(sortedSpawnPoint))
        {
             sortedSpawnPoint = PeopleSpawnPoints[Random.Range(0, PeopleSpawnPoints.Count)].transform.position; //Store the spawn point sorted
        }

        previousSortedPoint.Add(sortedSpawnPoint); //Add the spawn point sorted to the list of previously sorted spawn points
        return sortedSpawnPoint; //Return the sorted spawnpoint
    }

    //Clear the list of previouly sorted spawn points and current spawn point 
    private void OnDisable()
    {
        previousSortedPoint = new List<Vector2>(); //Clear the list of previouly sorted spawn points
        sortedSpawnPoint = new Vector2(); //Clear the current sorted spawn point
        visible = false; //Set visible to false
    }

    //Call spawn manager when object became visible
    private void OnBecameVisible()
    {
        //Check if it hasn't already became visible
        if (!visible)
        {
            SpawnManager.Instance.SpawnHouseEvent(); //Call spawn manager to spawn next house
            visible = true; //Set visible to true
        }
    }
}

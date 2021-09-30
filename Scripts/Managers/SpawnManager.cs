using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] private List<ObjectPool> housePools; //List of house pools
    [SerializeField] private List<ObjectPool> characterPools; //List of character pools
    [SerializeField] private ObjectPool hpPool; // Hp pool
    [SerializeField] private float spawnRateCharacterMax; //Spawn rate for the characters
    [SerializeField] private float spawnRateCharacterMin; //Spawn rate limit for the characters
    [SerializeField] private float spawnRateHP; //Spawn rate of the HP
    [SerializeField] private float hpMaxYPosition; //Y max position to spawn the HP
    [SerializeField] private float hpMinYPosition; //Y min position to spawn the HP
    private GameObject personToSpawn; //Character that will be spawned
    private int previousSpawnedHouse; //Index of the previously spawned house
    private int previousSpawnedCharacter; //Index of the previously spawned character
    private bool started; //If the coroutines have already started
    int activeCharacter = 0; //Index of the character
    int activeHouse = 0; //Index of the house

    //Subscribe the method StartCoroutines for the event OnChangedState
    private void OnEnable()
    {
        GameManager.OnChangedState += StartCoroutines; //Subscribe the method StartCoroutines for the event OnChangedState
    }

    //Unsubscribe the method StartCoroutines for the event OnChangedState
    private void OnDisable()
    {
        GameManager.OnChangedState -= StartCoroutines; //Unsubscribe the method StartCoroutines for the event OnChangedState
    }

    //Spawn characters on the ground
    private IEnumerator SpawnPersonOverTime()
    {
        started = true; //Set that the coroutine have started

        //Loop that will spawn the characters as long as the game is running
        while (GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING)
        {
            GetCharacterToSpawn(); //Call the method to get an avaliable character
            personToSpawn.transform.position = personToSpawn.GetComponent<PeopleController>().SpawnPoint; //Set the character to it's initial position
            personToSpawn.SetActive(true); //Activate the character object

            yield return new WaitForSeconds(Random.Range((spawnRateCharacterMin - (MoveLeft.increaseSpeed / 5)), (spawnRateCharacterMax - (MoveLeft.increaseSpeed / 5)))); //Wait a random amount of seconds to repeat the loop
        }

        started = false; //Set that the coroutine have stoped
    }

    private IEnumerator SpawnHP()
    {
        WaitForSeconds delay = new WaitForSeconds(spawnRateHP); //Delay to continue
        started = true;//Set that the coroutine have started

        //Loop that will spawn the characters as long as the game is running
        while (GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING)
        {
            yield return delay; //Wait the delay

            GameObject special = hpPool.GetAvailableObject(); // Get a avaliable character from the pool using the sorted index
            special.transform.position = new Vector3(15, Random.Range(hpMinYPosition, hpMaxYPosition), 0); //Set the character to it's initial position
            special.SetActive(true); //Activate the character object
        }

        started = false; //Set that the coroutine have stoped
    }

    //Get an avaliable character
    private void GetCharacterToSpawn()
    {
        //Loop that will select a different character
        while (activeCharacter == previousSpawnedCharacter)
        {
            activeCharacter = Random.Range(0, characterPools.Count); //Get an index for which character will be spawned
        }

        previousSpawnedCharacter = activeCharacter; //Set the previous character index
        personToSpawn = characterPools[activeCharacter].GetAvailableObject(); // Get a avaliable character from the pool using the sorted index
    }

    //Start the coroutines that spawn the houses and characters
    private void StartCoroutines(GameManager.GameState gameState)
    {
        //Check if the game is running
        if (gameState == GameManager.GameState.RUNNING)
        {
            //Check if the coroutines have already started
            if (!started)
            {
                StartCoroutine(SpawnHP()); //Start the coroutine to spawn the HP
                StartCoroutine(SpawnPersonOverTime()); //Start the coroutine to spawn the characters
            }
        }
    }

    //Spawn the houses
    public void SpawnHouseEvent()
    {
        //Loop that will select a different house
        while (activeHouse == previousSpawnedHouse)
        {
            activeHouse = Random.Range(0, housePools.Count); //Get an index for which house will be spawned
        }

        previousSpawnedHouse = activeHouse; //Set the previous house index
        GameObject houseToSpawn = housePools[activeHouse].GetAvailableObject(); //Get a avaliable house from the pool using the sorted index
        houseToSpawn.transform.position = houseToSpawn.GetComponent<HouseController>().SpawnPoint; //Set the house to it's initial position
        int numberOfPeopleToSpawn = Random.Range(1, houseToSpawn.GetComponent<HouseController>().PeopleSpawnPoints.Count); //Get a number of characters to be spawned on the house plataforms
        houseToSpawn.SetActive(true); //Activate the house object

        //Loop that will spawn the sorted number of characters on the house plataforms
        for (int i = 1; i <= numberOfPeopleToSpawn; i++)
        {
            GetCharacterToSpawn(); //Call the method to get an avaliable character
            personToSpawn.transform.position = houseToSpawn.GetComponent<HouseController>().GetPeopleSpawnPoint(); //Set the character to it's initial position
            personToSpawn.SetActive(true); //Activate the character object
        }
    }
}

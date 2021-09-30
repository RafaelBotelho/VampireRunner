using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundController : MonoBehaviour
{
    private int loops; //Number of loops performed
    private int activeBackground = 0; //Index of the background currently active
    private int activeGround = 0; //Index of the ground currently active
    private int previousBackground = 0; //Index of the background previously active
    private int previousGround = 0; //Index of the ground previously active
    [SerializeField] private float loopsToChange; //Number of loops necessary to change the background
    [SerializeField] private float xDestination; //Point to reset the position
    [SerializeField] private float xInitialization; //Point to inicialize the position
    [SerializeField] private List<Transform> backgrounds; //List of the backgrounds avaliable
    [SerializeField] private List<Transform> grounds; //List of grounds avaliable

    //Check what needs to be done with the backgrund
    private void Update()
    {
        //Check if it's time to reset the position
        if (backgrounds[activeBackground].position.x < xDestination)
        {
            loops++; //Increment the loops performed

            //Check if has performed the necessary loops to change
            if(loops > loopsToChange)
            {
                loops = 0; //Reset the loop count
                ChangeBackground(); //Call function to set a new background
            }
            //Reset the background position if the necessary amount of loops hasn't been reached
            else
            {
                backgrounds[activeBackground].position = Vector3.zero; //Reset the background position
            }
        }

        //Check if it's time to reset the ground position
        if (grounds[activeGround].position.x < xDestination)
        {
            grounds[activeGround].position = new Vector3(0, grounds[activeGround].position.y, 0); //Reset the ground position
        }

        //Check if the previous ground and background are active
        if (grounds[previousGround].gameObject.activeSelf || backgrounds[previousBackground].gameObject.activeSelf)
        {
            //Check if it's time to deactivate the previous ground
            if (grounds[previousGround].position.x < xDestination * 2)
            {
                grounds[previousGround].gameObject.SetActive(false); //Deactivate the previous ground
            }

            //Check if it's time to deactivate the previous background
            if (backgrounds[previousBackground].position.x < xDestination * 2) 
            {
                backgrounds[previousBackground].gameObject.SetActive(false); //Deactivate the previous background
            }
        }
    }

    //Change the current background
    private void ChangeBackground()
    {
        previousBackground = activeBackground; //Store the previous background
        previousGround = activeGround; //Store the previous ground

        //Repeat until the background selected is diferent from the previous one
        while (activeBackground == previousBackground)
        {
            activeBackground = Random.Range(0, backgrounds.Count); //Get a random background
        }

        //Check if the background selected is the nature
        if (activeBackground == 1)
        {
            activeGround = 1; //Set the nature ground 
        }
        //Set the stone ground if it's not the nature background
        else
        {
            activeGround = 0; //Set the stone ground
        }

        backgrounds[activeBackground].position = new Vector3(xInitialization, backgrounds[activeBackground].position.y, backgrounds[activeBackground].position.z); //Set the background in position

        //Check if the selected ground is diferent from the previous one
        if (activeGround != previousGround)
        {
            grounds[activeGround].position = new Vector3(xInitialization, grounds[activeGround].position.y, grounds[activeGround].position.z); //Set the ground in position
            grounds[activeGround].GetComponent<TilemapRenderer>().sortingOrder++; //Set the current ground to be on top of the previous
        }

        backgrounds[activeBackground].gameObject.SetActive(true); //Activate the background
        grounds[activeGround].gameObject.SetActive(true);  //Activate the ground
    }
}
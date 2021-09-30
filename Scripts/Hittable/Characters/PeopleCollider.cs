using UnityEngine;

public class PeopleCollider : MonoBehaviour
{
    public HittableObject hittableData; //Reference to the object data
    private bool hit; //If the object has been hitted already

    //Detect if the character has collided with the player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check if the object hasn't been hitted
        if (!hit)
        {
            //Check if the character has collided with the player
            if (collision.gameObject.CompareTag("Player"))
            {
                hit = true; //Set that the object has been hitted
                gameObject.GetComponent<BoxCollider2D>().enabled = false; //Disable the box collider
                gameObject.GetComponentInParent<Animator>().Play(hittableData.hittableAnimation); //Play the animation
            }
        }
    }

    //Enable the box collider when object is enabled
    private void OnEnable()
    {
        hit = false; //Set hitted to false
        gameObject.GetComponent<BoxCollider2D>().enabled = true; //Enable the box collider
    }
}

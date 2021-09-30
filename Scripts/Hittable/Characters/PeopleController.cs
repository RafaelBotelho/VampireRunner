using UnityEngine;

public class PeopleController : MonoBehaviour
{
    [SerializeField] private Vector2 spawnPoint; //Character spawn point information

    //Character spawn point  property
    public Vector2 SpawnPoint
    {
        get { return spawnPoint; } //Get the character spawn point stored
    }

    //Ignore collision with the vampire when enabled
    private void OnEnable()
    {
        Physics2D.IgnoreCollision(GameObject.Find("Vampire").GetComponent<Collider2D>(), GetComponent<Collider2D>(), true); //Ignore collision with the vampire
    }
}

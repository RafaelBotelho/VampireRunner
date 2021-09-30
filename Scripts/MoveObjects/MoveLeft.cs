using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    [SerializeField] private float speed; //Speed the object will be moved
    [SerializeField] private float xPositionLimit; //Limit position on the x axis
    public static float increaseSpeed; //Static variable that will be used to increase the speed of the objects over time
    private Transform transformVar; //Transform of the game object

    //Store the transform of the object at the start of the game
    private void Start()
    {
        transformVar = transform; //Store the transform of the object
    }
    //Update the position of the object moving it left
    private void Update()
    {
        transformVar.Translate(Vector2.left * (speed + increaseSpeed) * Time.deltaTime); //Move the object to the left

        //Check if the object is not the background
        if (!gameObject.CompareTag("BackGround"))
        {
            //Check if the current position reached the screen limit
            if (transformVar.position.x <= xPositionLimit)
            {
                gameObject.SetActive(false); //Deactivate the object
            }
        }
    }
}

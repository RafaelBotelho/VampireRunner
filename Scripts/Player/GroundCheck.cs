using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private Rigidbody2D playerRB; //Reference to the player Rigidbody2D 
    private PlayerController playerController; //Reference to the player controller
    [SerializeField] private Animator dustAnimator; //Reference to the dust animator of the player

    //Set the references at the start
    private void Start()
    {
        playerRB = GetComponentInParent<Rigidbody2D>(); //Set the player Rigidbody2D
        playerController = GetComponentInParent<PlayerController>(); //Set the player controller
    }

    //Check if the player is grounded 
    private void OnCollisionStay2D(Collision2D collision)
    {
        //Check if the player has collided with the ground and is falling
        if (collision.gameObject.CompareTag("Ground") && playerRB.velocity.y <= 0 && GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING)
        {
            playerController.Grounded(); //Set the player as grounded
        }
    }

    //Check if the player left the ground
    private void OnCollisionExit2D(Collision2D collision)
    {
        //Check if the player left the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            playerController.GetComponent<Animator>().SetBool("Air", true); //Set that the player is in the air
            dustAnimator.Play("New State"); //Disable the dust from the screen
        }
    }
}

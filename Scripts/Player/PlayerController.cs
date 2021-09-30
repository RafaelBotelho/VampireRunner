using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public delegate void Hit(int score, int hp, Color vignetteColor); //Delegate for when the player collide with an enemy

    public static event Hit OnHitted; //Event for when the player collide with an enemy

    [SerializeField] private float jumpForce; //Player jump force
    [SerializeField] private AudioClip jumpSFX; //Audio clip that will be played when the vampire jump
    [SerializeField] private AudioClip doubleJumpSFX; //Audio clip that will be played when the vampire double jump
    [SerializeField] private AudioClip deathSFX; //Audio clip that will be played when the vampire die
    [SerializeField] private GameUIController gameUIController; //Reference to the game UI controller
    [SerializeField] private Animator dustAnimator; //Reference to the dust animator
    private bool doubleJump; //Variable that specify if the player can double jump or not
    private bool jump; //Variable that specify if the player can jump or not
    private Rigidbody2D playerRB; //Reference to the player rigidbody 2D
    private Animator playerAnim; //Reference to the player animator

    //Subscribe to events when enabled
    private void OnEnable()
    {
        GameManager.OnChangedState += Die; //Subscribe the method Die to the event OnChangedState
    }

    //Unsubscribe to events when enabled
    private void OnDisable()
    {
        GameManager.OnChangedState -= Die; //Unsubscribe the method Die to the event OnChangedState
    }

    //Get references at the start of the game
    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>(); //Get a reference to the player Rigidbody 2D 
        playerAnim = GetComponent<Animator>(); //Get a reference to the player Animator
    }

    //Check for input player input every frame
    private void Update()
    {
        //Check if the player can jump
        if (Input.GetMouseButtonDown(0) && GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING && !IsPointerOverUIObject())
        {
            Jump(); //Call the function to make the player jump
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    //Check for collisions with hittable objects
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check if the collided with a hittable object
        if (collision.gameObject.CompareTag("Hittable"))
        {
            HittableObject data = collision.gameObject.GetComponent<PeopleCollider>().hittableData; //Get the data from that object
            AudioManager.Instance.PlaySFX(2, data.hitClip); //Play the apropriate audio clip
            playerAnim.Play(data.vampireAnimation); //Play the apropriate animation

            //Fire event OnHitted
            if (OnHitted != null)
            {
                OnHitted(data.blood, data.hp, data.vignetteColor);
            }
        }
    }

    //Check witch jump
    private void Jump()
    {
        //Check if the player can double jump
        if (jump && !doubleJump)
        {
            MakeJump(doubleJumpSFX, "Vampire_Double_Jump"); //Call function to execute the jump
            doubleJump = true; //Disable the double jump option
        }

        //Check if the player can jump
        if (!jump)
        {
            MakeJump(jumpSFX, "Vampire_Jump"); //Call function to execute the jump
            jump = true; //Disable the jump option
        }
    }

    //Execute the jump
    private void MakeJump(AudioClip clip, string animationName)
    {
        AudioManager.Instance.PlaySFX(1, clip); //Call the Audio Manager to play the jump audio clip;
        playerAnim.Play(animationName); //Play the jump animation
        playerRB.velocity = Vector2.up * jumpForce; //Make the player jump
        playerAnim.SetBool("Air", true); //Set that the vampire is on the air for the animator
    }

    //Play the Die animation when the game is over
    private void Die(GameManager.GameState gameState)
    {
        //Check if is game over
        if (gameState == GameManager.GameState.GAMEOVER)
        {
            AudioManager.Instance.PlaySFX(2, deathSFX); //Call the Audio Manager to play the death audio clip;
            playerAnim.updateMode = AnimatorUpdateMode.UnscaledTime; //Set the animator of the vampire to unscaled time
            playerAnim.Play("Vampire_Death"); //Play the death animation
        }
    }

    //Call the game over panel at the end of the death animation
    public void CallGameOverPanel()
    {
        gameUIController.ShowContinue(); //Call the game over panel
    }

    //Set the type of the animator to normal at the end of the death animation
    public void ChangeScaleAnimation()
    {
        playerAnim.updateMode = AnimatorUpdateMode.Normal; //Update the animator
    }

    //Set the player as grounded
    public void Grounded()
    {
        playerAnim.SetBool("Air", false); //Set that the vampire isn't in the air for the animator
        dustAnimator.Play("Dust_Animation"); //Play the dust animation
        jump = false; //Enable for the player to jump
        doubleJump = false; //Enable for the player to double jump
    }
}

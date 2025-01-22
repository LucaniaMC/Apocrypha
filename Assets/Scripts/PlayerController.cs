using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]

    private const float dashCooldown = 0.5f;    // Time between dash, so the player doesn't dash too much on ground
    private const float dashTime = 0.1f;        // How long do dashes last
    [Space]

    [Header("Assists")]

    [SerializeField] private float dashBuffer = 0.1f;   // Grace time allowed for player dash input before it can dash
    [SerializeField] private float jumpBuffer = 0.1f;   // Grace time allowed for player jump input before being grounded
    [Space]

    [Header("References")]

	public PlayerMovement movement;
	public PlayerAttack attack;

    [Header("Events")]
	[Space]

	public UnityEvent OnDashEvent;          //Functions to call when the player dashes
	public UnityEvent OnDashEndingEvent;	//Functions to call when the dash ends
    public UnityEvent OnDashRefillEvent;	//Functions to call when the dash cooldown ends
    public UnityEvent OnAttackEvent;        //Functions to call when the player lands

    [System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


    //Readable Variables
    public float horizontalMoveInput {get; private set;} = 0f;    // Input for horizontal movement, on a range of -1 (left) to 1 (right)
    public bool jumpInput {get; private set;} = false;            // Set to true after user input, then back to false once the player has jumped
    public bool wallJumpInput {get; private set;} = false;        // Set to true after user input, then back to false once the player has jumped
    public bool sitting {get; private set;} = false;              // Whether the player is sitting, toggle with input
    public bool dashInput {get; private set;} = false;            // Set to true after user input, then set back to false after dashTime

    //Timers
    float jumpBufferCounter = 0f;   //Countdown timer for jump buffering
    float dashBufferCounter = 0f;   //Same timer for dash

    //Private variables
    bool dashOnCooldown = false;    //Dash conditions to prevent dashing without cooldown on
    bool canDash = false;           //Can the player dash


    void Update()
    {
        //Get player Horizontal Movement, in Update function for percise input
        horizontalMoveInput = Input.GetAxisRaw("Horizontal");


        //Jump buffer timer
        if (Input.GetButtonDown("Jump")) 
        {
            jumpBufferCounter = jumpBuffer;
        } 
        else 
        {   //Same as coyote time counter
            jumpBufferCounter = Mathf.Clamp(jumpBufferCounter - Time.deltaTime, 0f, jumpBuffer);
        }


        //No jump during dash
        if (!dashInput) 
        {
            //Jump
            if (jumpBufferCounter > 0f && movement.coyoteTimeCounter > 0f) //Jump buffer counter acts as jump input check, and coyote time counter acts as ground check
            //Additional ground check so the animator trigger won't be triggered in midair. 
            {
                jumpInput = true; // Inputs are used in FixedUpdate

                jumpBufferCounter = 0f; //reset jump buffer time immediately after jumping
            } 
            else if (jumpBufferCounter > 0f && movement.wallCoyoteTimeCounter > 0f && movement.grounded == false) //Wall jump
            {
                wallJumpInput = true;

                jumpBufferCounter = 0f; //reset
            }
        }

        
        //Jump cutting, lower vertical velocity if the player releases jump button early
        //If the player jumped only in buffer time and isn't holding down the jump key, also lower vertical velocity
        if (!Input.GetButton("Jump") && movement.rb.velocity.y > 0f && movement.isJumping == true) //only works if the player is gaining height from jumping
        {
            movement.JumpCut();
        }


        //Sit
        if(Input.anyKeyDown && movement.grounded == true) //Put everything inside anykeydown and make an exception for the sit key
        {       
            if (Input.GetKeyDown(KeyCode.X)) //when sit key is pressed, sit down if standing, and stand up if sitting
            {
                sitting = !sitting; 
            } 
            else //If keys other than the sit key is pressed, stand up
            {
                sitting = false;
            }
        }


		//Attack
        if (Input.GetMouseButtonDown(0) && attack.attacking == false && dashInput == false) //No attack during dash
        {
            OnAttackEvent.Invoke();
			StartCoroutine(attack.Attack());
        }


        //Dash buffer timer
        if (Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            dashBufferCounter = dashBuffer;
        } 
        else 
        {   //Same as coyote time counter
            dashBufferCounter = Mathf.Clamp(dashBufferCounter - Time.deltaTime, 0f, dashBuffer);
        }


        //Dash
        if (dashBufferCounter > 0f && canDash == true && dashOnCooldown == false && movement.onWall == false && movement.knockbackTimeCounter == 0f) //No dash on wall or during knockback
            {
                StartCoroutine(Dash());
                dashBufferCounter = 0f; //reset
                canDash = false;
            }
        //Resets dash if grounded or on wall
        if (movement.grounded || movement.onWall)
        {
            canDash = true;
        }
    }


    void FixedUpdate()
    {
        //Move player
        movement.Move(horizontalMoveInput);
        movement.Jump(jumpInput);
        movement.WallJump(wallJumpInput);
        movement.Dash(dashInput);

        jumpInput = false; //Reset jump after jumped
        wallJumpInput = false; //reset wall jump
    }


    //Dash sequence
    private IEnumerator Dash() 
    {
        if (dashOnCooldown == false && canDash == true) 
        {
            //Start of Dash
            dashInput = true;
            OnDashEvent.Invoke();

            yield return new WaitForSeconds(dashTime);

            //End of dash
            dashOnCooldown = true;
            dashInput = false;
            OnDashEndingEvent.Invoke();
            movement.DashReset();

            yield return new WaitForSeconds(dashCooldown);

            //Dash refill, can dash again
            dashOnCooldown = false;
            OnDashRefillEvent.Invoke();
        }  
    }
}
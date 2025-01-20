using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]

    private const float dashCooldown = 0.5f;        //Time between dash, so the player doesn't dash too much on ground
    private const float dashTime = 0.1f;            //How long do dashes last
    [Space]

    [Header("Assists")]

    [SerializeField] private float dashBuffer = 0.1f;   //time allowed for player input for dash
    [SerializeField] private float jumpBuffer = 0.1f;   //time allowed for player jump input
    [Space]

    [Header("References")]

	public PlayerMovement movement;
	public PlayerAttack attack;

    [Header("Events")]
	[Space]

	public UnityEvent OnDashEvent;	    //Functions to call when the player dashes
	public UnityEvent OnDashEndingEvent;	//Functions to call when the dash ends
    public UnityEvent OnDashRefillEvent;	//Functions to call when the dash cooldown ends
    public UnityEvent OnAttackEvent;	//Functions to call when the player lands

    [System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


    //Variables
    [HideInInspector] public float horizontalMoveInput = 0f;
	[HideInInspector] public bool jumpInput = false;
    [HideInInspector] public bool wallJumpInput = false;
    [HideInInspector] public bool sitting = false;
    [HideInInspector] public bool dashInput = false;

    //Timers
    float jumpBufferCounter = 0f; //Countdown timer for jump buffering
    float dashBufferCounter = 0f;   //Same timer for dash

    //States
    bool isDashing = false; //Dash conditions to prevent dashing without cooldown on
    bool canDash = false;   //Can the player dash


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
            if (jumpBufferCounter > 0f && movement.coyoteTimeCounter > 0f)
            //Additional ground check so the animator trigger won't be triggered in midair, it stops working when it happens. 
            //There's also a grounded condition check in the animator from any state to jump to fix the issue. 
            //Edit: Replaced grounded check with coyote timer, and replaced jump button with jump buffer counter
            {
                jumpInput = true; //The actual jump part is in FixedUpdate because tutorial said so

                jumpBufferCounter = 0f; //reset jump buffer time immediately after jumping
            } 
            else if ((jumpBufferCounter > 0f) && movement.wallCoyoteTimeCounter > 0f && movement.grounded == false) //Wall jump
            {
                wallJumpInput = true;

                jumpBufferCounter = 0f; //reset
            }
        }

        
        //Variable jump, lower vertical velocity if the player releases jump button early
        //If the player jumped only in buffer time and isn't holding down the jump key, also lower vertical velocity
        if (!Input.GetButton("Jump") && movement.rb.velocity.y > 0f && movement.isJumping == true) //only works if the player is gaining height from jumping
        {
            movement.VariableJump();
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
        if (dashBufferCounter > 0f && canDash == true && isDashing == false && movement.onWall == false && movement.knockbackTimeCounter == 0f) //No dash on wall or during knockback
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
        movement.Move(horizontalMoveInput * Time.fixedDeltaTime, jumpInput, wallJumpInput, dashInput);
        jumpInput = false; //Reset jump after jumped
        wallJumpInput = false; //reset wall jump
    }


    //Dash sequence
    private IEnumerator Dash() 
    {
        if (isDashing == false && canDash == true) 
        {
            isDashing = true;
            dashInput = true;
            OnDashEvent.Invoke();
            yield return new WaitForSeconds(dashTime);
            dashInput = false;
            OnDashEndingEvent.Invoke();
            yield return new WaitForSeconds(dashCooldown);
            isDashing = false;
            OnDashRefillEvent.Invoke();
        }  
    }
}
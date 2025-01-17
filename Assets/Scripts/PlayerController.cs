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
    public Animator animator;       //Animator for player sprite
	public Animator trailAnimator;  //animator for attack trail

    [Header("Events")]
	[Space]

	public UnityEvent OnDashEvent;	    //Functions to call when the player dashes
	public UnityEvent OnDashEndingEvent;	//Functions to call when the dash ends
    public UnityEvent OnDashRefillEvent;	//Functions to call when the dash cooldown ends

    [System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


    //Variables
    float horizontalMove = 0f;
	[HideInInspector] public bool jump = false;
    [HideInInspector] public bool wallJump = false;
    [HideInInspector] public bool sit = false;
    [HideInInspector] public bool dash = false;

    //Timers
    float jumpBufferCounter = 0f; //Countdown timer for jump buffering
    float dashBufferCounter = 0f;   //Same timer for dash

    //States
    bool isDashing = false; //Dash conditions
    bool canDash = false;   //Can the player dash

    private bool altAttack = false; //Choose which random attack animation to play


    void Update()
    {
        //Get player Horizontal Movement, in Update function for percise input
        horizontalMove = Input.GetAxisRaw("Horizontal");


        //Animator parameters
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        animator.SetBool("IsGrounded", movement.grounded);
        animator.SetFloat("FallTime", movement.fallTime);
        animator.SetFloat("VerticalVelocity", movement.rb.velocity.y);
        animator.SetBool("OnWall", movement.onWall);
        animator.SetBool("Dash", dash);


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
        if (!dash) 
        {
            //Jump
            if (jumpBufferCounter > 0f && movement.coyoteTimeCounter > 0f)
            //Additional ground check so the animator trigger won't be triggered in midair, it stops working when it happens. 
            //There's also a grounded condition check in the animator from any state to jump to fix the issue. 
            //Edit: Replaced grounded check with coyote timer, and replaced jump button with jump buffer counter
            {
                jump = true; //The actual jump part is in FixedUpdate because tutorial said so
                animator.SetTrigger("Jump");

                jumpBufferCounter = 0f; //reset jump buffer time immediately after jumping
            } 
            else if ((jumpBufferCounter > 0f) && movement.wallCoyoteTimeCounter > 0f && movement.grounded == false) //Wall jump
            {
                wallJump = true;
                animator.SetTrigger("Jump");

                jumpBufferCounter = 0f; //reset
            }
        }


        //Variable jump, lower vertical velocity if the player releases jump button early
        //If the player jumped only in buffer time and isn't holding down the jump key, also lower vertical velocity
        if (Input.GetButtonUp("Jump") && movement.rb.velocity.y > 0f || !Input.GetButton("Jump") && movement.rb.velocity.y > 0f)
        {
            movement.VariableJump();
        }


        //Sit
        if(Input.anyKeyDown && movement.grounded == true) //Put everything inside anykeydown and make exceptions, otherwise it's janky
        {       
            if (Input.GetKeyDown(KeyCode.X)) //when sit key is pressed, sit down if standing, and stand up if sitting
            {
                if (sit == false) 
                {
                    animator.SetTrigger("Sit");
                    animator.SetBool("IsSitting", true);
                    sit = true;
                } 
                else if (sit == true) 
                {
                    animator.SetTrigger("Stand");
                    animator.SetBool("IsSitting", false);
                    sit = false;
                }  
            } 
            else //If keys other than the sit key is pressed, stand up without standing animation
            {
                animator.SetBool("IsSitting", false);
                sit = false;
            }
        }


		//Attack
        if (Input.GetMouseButtonDown(0) && attack.attacking == false && dash == false) //No attack during dash
        {
            animator.SetTrigger("Attack");
            trailAnimator.SetTrigger("Attack");
			StartCoroutine(attack.Attack());

            //Alternative attack animator
            animator.SetBool("AltAttack", altAttack);
            trailAnimator.SetBool("AltAttack", altAttack);
            altAttack = !altAttack;
        }
        //Stops trail attack animation if attack is interrupted due to flipping
        trailAnimator.SetBool("Attacking", attack.attacking);


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
        if (dashBufferCounter > 0f && canDash == true && isDashing == false && movement.onWall == false && movement.knockbackControl >= 1f) //No dash on wall or during back
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
        movement.Move(horizontalMove * Time.fixedDeltaTime, jump, wallJump, dash);
        jump = false; //Reset jump after jumped
        wallJump = false; //reset wall jump
    }


    //Dash sequence
    private IEnumerator Dash() 
    {
        if (isDashing == false && canDash == true) 
        {
            isDashing = true;
            dash = true;
            OnDashEvent.Invoke();
            yield return new WaitForSeconds(dashTime);
            dash = false;
            OnDashEndingEvent.Invoke();
            yield return new WaitForSeconds(dashCooldown);
            isDashing = false;
            OnDashRefillEvent.Invoke();
        }  
    }
}
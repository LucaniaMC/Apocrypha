using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]

    [SerializeField] private float runSpeed = 500f;
    [SerializeField] private float jumpBuffer = 0.1f; //time allowed for player jump input


    [Header("References")]

    public Animator animator;
	public PlayerMovement movement;
	public PlayerAttack attack;
	public Animator trailAnimator; //animator for attack trail


    //Variables
    float horizontalMove = 0f;
	[HideInInspector] public bool jump = false;
    [HideInInspector] public bool wallJump = false;
    [HideInInspector] public bool sit = false;
    [HideInInspector] public bool dash = false;

    float jumpBufferCounter = 0f; //Countdown timer for jump buffering

    private bool altAttack = false; //Choose which random attack animation to play

    const float dashTime = 0.1f; //How long the player can dash
    float dashTimer = 0f;   //Countdown time for dash
    bool canDash = true;    //Is the player able to dash


    void Update()
    {
        //Get player Horizontal Movement
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

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
        if (Input.GetMouseButtonDown(0) && attack.attacking == false)
        {
            animator.SetTrigger("Attack");
            trailAnimator.SetTrigger("Attack");
			StartCoroutine(attack.Attack());

            //Alternative attack animator
            animator.SetBool("AltAttack", altAttack);
            trailAnimator.SetBool("AltAttack", altAttack);
            altAttack = !altAttack;
        }

        //Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash == true)
            {
                //Starts timer when dash key is pressed
                dashTimer = dashTime;
                canDash = false;
            }
        //Dash timer
        dashTimer = Mathf.Clamp(dashTimer - Time.deltaTime, 0f, dashTime);
        //Stops dash if the player touches wall
        if (movement.onWall) 
        {
            dashTimer = 0f;
        }
        //Sets dash
        if(dashTimer > 0) 
        {
            dash = true;
        } 
        else 
        {
            //Dash stops when the counter reached 0
            dash = false;
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
}
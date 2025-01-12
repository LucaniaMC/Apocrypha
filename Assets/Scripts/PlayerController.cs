using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Parameters")]

    [SerializeField] private float runSpeed = 500f;


    [Header("References")]

    public Animator animator;
	public PlayerMovement movement;
	public PlayerAttack attack;
	public Animator trailAnimator; //animator for attack trail


    //Variables
    float horizontalMove = 0f;
	[HideInInspector] public bool jump = false;
    [HideInInspector] public bool sit = false;

    private bool altAttack = false; //Choose which random attack animation to play


    void Update()
    {
        //Get player Horizontal Movement
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        //Animator parameters
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        animator.SetBool("IsGrounded", movement.grounded);
        animator.SetFloat("FallTime", movement.fallTime);

        //Jump
        if (Input.GetButtonDown("Jump") && movement.coyoteTimeCounter > 0f) 
        //Additional ground check so the animator trigger won't be triggered in midair, it stops working when it happens. 
        //There's also a grounded condition check in the animator from any state to jump to fix the issue. 
        //Edit: Replaced grounded check with coyote timer
        {
            jump = true; //The actual jump part is in FixedUpdate because tutorial said so
            animator.SetTrigger("Jump");
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
    }


    void FixedUpdate()
    {
        //Move player
        movement.Move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false; //Reset jump after jumped
    }
}
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;       //Animator for player sprite
	public Animator trailAnimator;  //animator for attack trail

    public PlayerMovement movement;
	public PlayerAttack attack;
    public PlayerController controller;

    private bool altAttack = false; //Choose which random attack animation to play


    void Update()
    {
        //Animator parameters
        animator.SetBool("IsJumping", movement.isJumping);
        animator.SetFloat("Speed", Mathf.Abs(controller.horizontalMove));
        animator.SetBool("IsGrounded", movement.grounded);
        animator.SetFloat("FallTime", movement.fallTime);
        animator.SetFloat("VerticalVelocity", movement.rb.velocity.y);
        animator.SetBool("Attacking", attack.attacking);
        animator.SetBool("OnWall", movement.onWall);
        animator.SetBool("Dash", controller.dash);
        animator.SetBool("IsSitting", controller.sit);
        trailAnimator.SetBool("Attacking", attack.attacking);
    }


    //Turning animation, called in PlayerController's OnFlipEvents
    public void TurnAnimation() 
    {
        if (movement.grounded == true) //only turn if grounded
        {
            animator.SetTrigger("Turn");
        }
    }


    //Attack animation, called in PlayerController's OnAttackEvents
    public void AttackAnimation()
    {
        animator.SetTrigger("Attack");
        trailAnimator.SetTrigger("Attack");

        //Alternative attack animator
        altAttack = !altAttack;
        animator.SetBool("AltAttack", altAttack);
        trailAnimator.SetBool("AltAttack", altAttack);
    }
}
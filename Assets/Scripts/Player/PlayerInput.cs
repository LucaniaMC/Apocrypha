using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerData data;

    //Movement
    public float moveInput {get; private set;}
    public bool jumpInput {get; private set;}
    public bool jumpHoldInput {get; private set;}
    public bool dashInput {get; private set;}
    //Attack
    public bool attackInput {get; private set;}
    public bool attackHoldInput {get; private set;}
    public bool attackReleaseInput {get; private set;}


    //Timers
    private float lastJumpInputTime = float.NaN; //NaN prevents the player from jumping on initialization
    private float attackHoldStartTime = 0f;
    private bool isHolding = false; //If the player is charing up attack
    private float heldTime = 0f;


    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButtonDown("Jump");
        jumpHoldInput = Input.GetButton("Jump");
        dashInput = Input.GetKeyDown(KeyCode.LeftShift);

        attackInput = Input.GetMouseButtonDown(0);
        attackHoldInput = Input.GetMouseButton(0);
        attackReleaseInput = Input.GetMouseButtonUp(0);

        if(jumpInput) //Timer for jump buffer
        {
            lastJumpInputTime = Time.time;
        }

    }

    #region Jump Buffer
    //Allows the player to input a bit early, and the jump would still register when possible. Returns true if the jump buffer is active
    public bool JumpBuffer()
    {
        return Time.time - lastJumpInputTime <= data.jumpBuffer;
    }

    //prevents jumping multiple times during jump buffer
    public void ResetJumpBuffer() 
	{
		lastJumpInputTime = float.NaN;
	}
    #endregion


    #region Charge Attack
    //Returns true if the player held down the attack button for long enough then release
    public bool ChargeAttack()
    {
        // When the button is first pressed
        if (attackInput)
        {
            isHolding = true;
            attackHoldStartTime = Time.time;
        }

        if (attackHoldInput) 
        {
            heldTime = Time.time - attackHoldStartTime;
        }

        // When the button is released
        if (attackReleaseInput && isHolding)
        {
            isHolding = false; // Reset flag

            if (heldTime >= data.attackChargeTime)
            {
                heldTime = 0f; // Reset timer
                return true;
            }
            heldTime = 0f; // Reset timer even if not held long enough
        }
        return false;
    }

    //returns true if the player held down the attack button for long enough
    public bool CheckAttackHold()
    {
        // If holding, check if the hold time has been reached
        if (isHolding && (heldTime >= data.attackChargeTime))
        {
            return true;
        }
        return false;
    }
    #endregion
}

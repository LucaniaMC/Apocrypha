using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerData data;

    //Timers
    private float lastJumpInputTime = float.NaN; //NaN prevents the player from jumping on initialization
    private float attackHoldStartTime = float.NaN;
    private float heldTime = 0f;    //How long has the player been holding down attack


    void Update()
    {
        if(JumpInput()) //Timer for jump buffer
            lastJumpInputTime = Time.time; 
    }


    #region Movement Inputs
    public float MoveInput() //returns 1 if moving right, -1 if moving left, and 0 if not moving
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public float VerticalInput() //returns 1 for up, -1 for down, and 0 for not input
    {
        return Input.GetAxisRaw("Vertical");
    }

    public bool UpInput() 
    {
        return Input.GetAxisRaw("Vertical") == 1;
    }

    public bool DownInput() 
    {
        return Input.GetAxisRaw("Vertical") == -1;
    }

    public bool JumpInput() 
    {
        return Input.GetButtonDown("Jump");
    }

    public bool JumpHoldInput() 
    {
        return Input.GetButton("Jump");
    }

    public bool DashInput() 
    {
        return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
    }

    public bool SitInput() 
    {
        return Input.GetKeyDown(KeyCode.X);
    }
    #endregion


    #region Combat Inputs
    public bool AttackInput() 
    {
        return Input.GetMouseButtonDown(0);
    }

    public bool AttackHoldInput() 
    {
        return Input.GetMouseButton(0);
    }

    public bool AttackReleaseInput() 
    {
        return Input.GetMouseButtonUp(0);
    }

    public bool SecondaryAttackInput() 
    {
        return Input.GetMouseButtonDown(1);
    }

    public bool SecondaryAttackHoldInput() 
    {
        return Input.GetMouseButton(1);
    }

    public bool SecondaryAttackReleaseInput() 
    {
        return Input.GetMouseButtonUp(1);
    }
    #endregion


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
    // Returns true if the player held down the attack button for long enough
    public bool CanChargeAttack()
    {
        bool canChargeAttack = false;
        // When the button is first pressed, set timer start time
        if (AttackInput())
        {
            attackHoldStartTime = Time.time;
        }
        // When the button is held, calculate timer
        if (AttackHoldInput()) 
        {
            heldTime = Time.time - attackHoldStartTime;
        }
        // If holding, check if the hold time has been reached
        if (heldTime >= data.attackChargeTime)
        {
            canChargeAttack = true;
        }
        // Resets timer when not holding
        if (!AttackHoldInput() && heldTime != 0f) 
        {
            heldTime = 0f;
        }
        return canChargeAttack;
    }

    public void CancelChargeAttack() 
    {
        attackHoldStartTime = float.NaN;
        heldTime = 0f;
    }
    #endregion
}

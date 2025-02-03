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

    public float lastJumpInputTime {get; private set;} = 0f;
    private float lastJumpInputTime = float.NaN; //NaN prevents the player from jumping on initialization


    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButtonDown("Jump");
        jumpHoldInput = Input.GetButton("Jump");
        dashInput = Input.GetKeyDown(KeyCode.LeftShift);
        attackInput = Input.GetMouseButtonDown(0);

        if(jumpInput) //Timer for jump buffer
        {
            lastJumpInputTime = Time.time;
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
}

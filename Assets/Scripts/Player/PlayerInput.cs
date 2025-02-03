using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerData data;

    public float moveInput {get; private set;}
    public bool jumpInput {get; private set;}
    public bool jumpHoldInput {get; private set;}
    public bool dashInput {get; private set;}
    public bool attackInput {get; private set;}

    public float lastJumpInputTime {get; private set;} = 0f;


    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButtonDown("Jump");
        jumpHoldInput = Input.GetButton("Jump");
        dashInput = Input.GetKeyDown(KeyCode.LeftShift);
        attackInput = Input.GetMouseButtonDown(0);

        if(jumpInput) 
        {
            lastJumpInputTime = Time.time;
        }
    }


    public bool JumpBuffer()
    {
        return Time.time - lastJumpInputTime <= data.jumpBuffer;
    }


    public void ResetJumpBuffer() 
	{
		lastJumpInputTime = 0f;
	}
}

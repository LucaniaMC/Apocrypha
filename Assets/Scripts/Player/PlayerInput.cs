using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerData data;

    public float moveInput {get; private set;}
    public bool jumpInput {get; private set;}
    public bool jumpHoldInput {get; private set;}

    public float jumpBufferCounter {get; private set;} = 0f;


    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetButtonDown("Jump");
        jumpHoldInput = Input.GetButton("Jump");
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Scriptable Objects / Player Data")]
//Create a new asset via Assets -> Create -> Scriptable Objects -> Player Data

public class PlayerData : ScriptableObject
{
    [Header("Movement")]
	public float runSpeed;			                	// Horizontal velocity when running
	[Range(0f, 1f)] public float movementSmoothing;	    // Smoothing factor for ground movement using SmoothDamp
	[Range(0f, 1f)] public float airMovementSmoothing;	// Smoothing factor for air movement using SmoothDamp


    [Header("Jump")]
	public float jumpHeight;                     // Maximum height when the player jumps
    public float wallJumpHeight;                 // Maximum height when the player wall jumps
	[Range(0f, 1f)] public float jumpCutRate;   // Multiplier for reducing vertical velocity if the jump button is released early


    [Header("Dash")]
	public float dashSpeed;     // Horizontal velocity when dashing
    public float dashCooldown;  // Time between dashes to limit ground dashing frequency
    public float dashTime;      // How long do dashes last


    [Header("Combat")]
    public int attackDamage;        // Damage dealt by the player's attack
    public float attackChargeTime;  // Time required to hold the attack button for a charged attack
    public float invincibleTime;    // Duration of invincibility after the player takes damage


	[Header("Assists")]
	public float coyoteTime;        // Time allowed for jumping after leaving the ground
	public float wallCoyoteTime;    // Time allowed for performing a wall jump after losing contact
    public float jumpBuffer;        // Grace period for jump input before landing

    
    [Header("Others")]
    public float slideVelocity;		//Vertical velocity during wall slide
	public float limitVelocity;		//Vertical velocity limit when falling
	public float gravity;           //Player's RigidBody2D gravity scale

}
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Scriptable Objects / Player Data")]
//Create a new asset from Assets -> Create -> Scriptable Objects -> Player Data

public class PlayerData : ScriptableObject
{
    [Header("Movement")]
	public float runSpeed;				// Player horizontal velocity when running
	[Range(0f, 1f)] public float movementSmoothing;	    // How much to smooth out the player's movement with Smoothdamp
	[Range(0f, 1f)] public float airMovementSmoothing;	// How much to smooth out the player's movement in air with Smoothdamp
    [Space]

    [Header("Jump")]
	public float jumpForce;     // Amount of force added when the player jumps
	[Range(0f, 1f)] public float jumpCutRate;   // Multiplier for the player's vertical velocity if jump button is released during jump
    [Space]

    [Header("Dash")]
	public float dashSpeed;     // Player horizontal velocity when dashing
    public float dashCooldown;  // Time between dash, so the player doesn't dash too much on ground
    public float dashTime;      // How long do dashes last
	[Space]

    [Header("Attack")]
    public float attackTime;        // How long the attack hitbox stay active
    public float attackCooldown;    // Cooldown time between attacks
    public int attackDamage;        // How much damage does the player deal
    [Space]

	[Header("Assists")]
	public float coyoteTime;        // In seconds, time allowed for player to jump after leaving the ground 
	public float wallCoyoteTime;    // In seconds, coyote time for wall jump 
    public float dashBuffer;        // Grace time allowed for player dash input before it can dash 
    public float jumpBuffer;        // Grace time allowed for player jump input before being grounded
	[Space]

    
    [Header("Others")]
    public float slideVelocity;		//Player's vertical velocity when sliding on
	public float limitVelocity;		//Player's vertical velocity limit when falling
	public float gravity;           //Player's gravity scale, used for dash, sets to 0 when dashing and back to

}
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Scriptable Objects / Player Data")]
//Create a new asset from Assets -> Create -> Scriptable Objects -> Player Data

public class PlayerData : ScriptableObject
{
    [Header("Movement")]
	public float runSpeed;			                	// Player horizontal velocity when running
	[Range(0f, 1f)] public float movementSmoothing;	    // How much to smooth out the player's movement with Smoothdamp
	[Range(0f, 1f)] public float airMovementSmoothing;	// How much to smooth out the player's movement in air with Smoothdamp


    [Header("Jump")]
	public float jumpHeight;                     // Maximum height when the player jumps
    public float wallJumpHeight;                 // Maximum height when the player wall jumps
	[Range(0f, 1f)] public float jumpCutRate;   // Multiplier for the player's vertical velocity if jump button is released during jump


    [Header("Dash")]
	public float dashSpeed;     // Player horizontal velocity when dashing
    public float dashCooldown;  // Time between dash, so the player doesn't dash too much on ground
    public float dashTime;      // How long do dashes last


    [Header("Combat")]
    public int attackDamage;        // How much damage does the player deal
    public float attackChargeTime;  // How long the player need to hold down attack button for charged attack
    public float invincibleTime;    // How long after the player takes damage does it remain invincible


	[Header("Assists")]
	public float coyoteTime;        // In seconds, time allowed for player to jump after leaving the ground 
	public float wallCoyoteTime;    // In seconds, coyote time for wall jump 
    public float jumpBuffer;        // Grace time allowed for player jump input before being grounded

    
    [Header("Others")]
    public float slideVelocity;		//Player's vertical velocity when sliding on walls
	public float limitVelocity;		//Player's vertical velocity limit when falling
	public float gravity;           //Player's gravity scale, used for dash, sets to 0 when dashing and back to

}
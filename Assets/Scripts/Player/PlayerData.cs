using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
	
	[SerializeField] private float runSpeed;				// Player horizontal velocity when running 10f
	[SerializeField] private float movementSmoothing;	// How much to smooth out the player's movement with Smoothdamp 0.05f
	[SerializeField] private float airMovementSmoothing;	// How much to smooth out the player's movement in air with Smoothdamp 0.1f
    [Space]

    [Header("Jump")]
	[SerializeField] private float jumpForce;			// Amount of force added when the player jumps 850f
	[SerializeField] private float jumpCutRate;			// Multiplier for the player's vertical velocity if jump button is released during jump 0.5f
    [Space]

    [Header("Dash")]
	[SerializeField] private float dashSpeed;				 // Player horizontal velocity when dashing 30f
    [SerializeField] private float dashCooldown;          // Time between dash, so the player doesn't dash too much on ground 0.5f
    [SerializeField] private float dashTime;              // How long do dashes last 0.1f
	[Space]

    [Header("Attack")]
    [SerializeField] private float attackTime;            // How long the attack hitbox stay active 0.2f
    [SerializeField] private float attackCooldown;        // Cooldown time between attacks 0.1f
    public int attackDamage;                                // How much damage does the player deal 10
    

	[Header("Assists")]

	[SerializeField] private float coyoteTime;		// In seconds, time allowed for player to jump after leaving the ground 0.1f
	[SerializeField] private float wallCoyoteTime;	// In seconds, coyote time for wall jump 0.2f
    [SerializeField] private float dashBuffer;       // Grace time allowed for player dash input before it can dash 0.1f
    [SerializeField] private float jumpBuffer;       // Grace time allowed for player jump input before being grounded 0.1f
	[Space]

    
    [SerializeField] private float slideVelocity;			//Player's vertical velocity when sliding on  -5f
	private float limitVelocity;			//Player's vertical velocity limit when falling 25f
	private float gravity;					//Player's gravity scale, used for dash, sets to 0 when dashing and back to 4

}
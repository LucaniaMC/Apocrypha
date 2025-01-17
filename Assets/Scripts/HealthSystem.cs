using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class HealthSystem : MonoBehaviour
{
    [Header("Parameters")]

    public int health = 100;            //The object's current health
    public int maxHealth = 100;         //The object's maximum health
    public float damageModifier = 1f;     //Multiply the damage taken, 0-1
    [Space]
    [SerializeField] private float invincibleTime = 0f;     //How long is the object invincible after taking damage 
    public bool isInvincible = false;                       //Is the object invincible
    [Space]
    public bool isDead = false;                             //Is the object dead


    [Header("Events")] 
	[Space]

    public UnityEvent OnDamageEvent;
	public UnityEvent OnDeathEvent;
    public UnityEvent OnHealEvent;


	[System.Serializable] 
	public class BoolEvent : UnityEvent<bool> { }


    //This script is shared by the player and enemies to give them a health system, damage and death functions and events
    //Since they'll have different behaviors when damaged and dead
    //It'll need another script specific to each object to call the functions and add in events

    void Start() //Not sure why
    {
        if (OnDeathEvent == null)
			OnDeathEvent = new UnityEvent();

        if (OnDamageEvent == null)
			OnDamageEvent = new UnityEvent();

        if (OnHealEvent == null)
			OnHealEvent = new UnityEvent();
    }


    //Damage function
    public void Damage(int damageAmount)
    {
        if (isDead || isInvincible) return; //No damage if the player's dead or invincible

        health -= (int)(damageAmount * damageModifier);
        OnDamageEvent.Invoke();

        StartCoroutine(Invincible(invincibleTime)); //Start invincibility time after taking damage
    
        Check();
    }


    //Heal function
    public void Heal(int healAmount)
    {
        health += (int)(healAmount);
        OnHealEvent.Invoke();
        Check();
    }


    void Check()
    {
        //Clamp health amount
        health = Mathf.Clamp(health, 0, maxHealth);

        //Death function
        if (health <= 0 && isDead == false) 
        {
            isDead = true;
            OnDeathEvent.Invoke();
        }
    }


    //Invincible time after damage
    private IEnumerator Invincible(float time) 
    {
        isInvincible = true;
        yield return new WaitForSeconds(time);
        isInvincible = false;
    }


    //Manually set invincibility, currently used in dash event
    public void SetInvincible(bool invincible) 
    {
        isInvincible = invincible;
    }
}

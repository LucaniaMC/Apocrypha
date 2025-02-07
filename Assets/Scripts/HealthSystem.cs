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


    //Damage function
    public void Damage(int damageAmount)
    {
        if (isDead || isInvincible) return; //No damage if the player's dead or invincible

        health -= (int)(damageAmount * damageModifier);
        OnDamageEvent.Invoke();

        if (invincibleTime > 0f)
        {
            SetInvincible(invincibleTime); //Start invincibility time after taking damage
        }
        Check();
    }


    //Heal function
    public void Heal(int healAmount)
    {
        health += (int)(healAmount);
        OnHealEvent.Invoke();
        Check();
    }


    //Check for player health clamp and death each time player health is changed
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


    //Manually set invincibility to true or false
    public void SetInvincible(bool invincible) 
    {
        isInvincible = invincible;
    }


    //Manually set invincibility with a set time during which the player is invincible
    public void SetInvincible(float time) 
    {
        StartCoroutine(Invincible(time));
    }


    //Corontine for invincible time after damage or manually set
    private IEnumerator Invincible(float time) 
    {
        isInvincible = true;
        yield return new WaitForSeconds(time);
        isInvincible = false;
    }
}

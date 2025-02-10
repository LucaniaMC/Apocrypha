using UnityEngine;
using UnityEngine.Events;
using System.Collections;

//This script is shared by the player and enemies to give them a health system, damage and death functions and events
public class HealthSystem : MonoBehaviour
{
    [Header("Parameters")]

    public int health = 100;            //The object's current health
    public int maxHealth = 100;         //The object's maximum health
    public float damageModifier = 1f;   //Multiply the damage taken, 0-1
    
    public bool isInvincible {get; private set;} = false;   //Is the object invincible
    public bool isDead {get; private set;} = false;         //Is the object dead

    private Coroutine activeInvisRoutine;    //active invincible coroutine

    [Header("Events")] 
	[Space]

    public UnityEvent OnDamageEvent;
	public UnityEvent OnDeathEvent;
    public UnityEvent OnHealEvent;


	[System.Serializable] 
	public class BoolEvent : UnityEvent<bool> { }

    
    #region Damage Functions
    public void Damage(int damageAmount)
    {
        if (isInvincible) return; //No damage if the player's dead or invincible

        health -= (int)(damageAmount * damageModifier);
        OnDamageEvent.Invoke();
        Check();
    }

    //Heal function
    public void Heal(int healAmount)
    {
        health += healAmount;
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
    #endregion


    #region Invincibility
    //Manually set invincibility to true or false
    public void SetInvincible(bool invincible) 
    {
        if(activeInvisRoutine != null)    //if there's a couritine active, stop it to prevent bugs
            StopCoroutine(activeInvisRoutine);

        isInvincible = invincible;
    }

    //Manually set invincibility with a set time during which the player is invincible
    public void SetInvincible(float time) 
    {
        //stop curretly active coroutine, and start a new one
        if(activeInvisRoutine != null) 
            StopCoroutine(activeInvisRoutine);

        activeInvisRoutine = StartCoroutine(Invincible(time));
    }

    //Corontine for SetInvincible with time input
    private IEnumerator Invincible(float time) 
    {
        isInvincible = true;
        yield return new WaitForSeconds(time);
        isInvincible = false;
    }
    #endregion
}

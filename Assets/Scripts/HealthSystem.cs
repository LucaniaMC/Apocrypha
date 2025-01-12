using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Parameters")]

    public int health = 100;            //The object's current health
    public int maxHealth = 100;         //The object's maximum health
    public float damageModifier = 1f;     //Multiply the damage taken, 0-1
    [Space]
    public bool isDead = false;


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
        health -= (int)(damageAmount * damageModifier);
        OnDamageEvent.Invoke();
    }


    //Heal function
    public void Heal(int healAmount)
    {
        health -= (int)(healAmount);
        OnHealEvent.Invoke();
    }


    //Death function
    void Update()
    {
        //Clamp health amount
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 0 && isDead == false) 
        {
            isDead = true;
            OnDeathEvent.Invoke();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;           //Main slider
    public Slider effectSlider;     //A second slider for smooth damage effect

    //Variables for damage effect, an effect that displays how much the player's health has decreased
    readonly float effectTimerMax = 1f;     //How long after not taking damage does the effect start
    float effectTimerCounter = 1f;
    readonly float effectSpeedMin = 20f;   //Minimum speed of the decrease effect
    float effectSpeed;                  //Current effect speed

    public HealthSystem health;


    void Start()
    {
        //Ensures slider positions are in place
        slider.maxValue = health.maxHealth;
        slider.value = health.health;
        effectSlider.value = slider.value;
    }

    
    void Update()
    {
        //Damage effect
        //The effect only start to decrease to the current health amount after a while, so the player can see how much damage they're taken
        effectTimerCounter = Mathf.Clamp(effectTimerCounter - Time.deltaTime, 0f, effectTimerMax);

        //Variable effect speed based on the difference between the slider value and effect slider value, with a minimum speed
        //Bigger difference means faster spped
        effectSpeed = Mathf.Max(effectSpeedMin, (effectSlider.value - slider.value) * 2);
        
        //Smoothly move the damage effect bar towards the current health bar position
        if (effectTimerCounter <= 0 && effectSlider.value > health.health) 
        {
            effectSlider.value = Mathf.MoveTowards(effectSlider.value, slider.value, effectSpeed * Time.deltaTime);
        }
    }


    //Called during any event that changes health value to update slider value
    public void SliderUpdate() 
    {
        slider.value = health.health;
    }


    //This is called in the player's OnDamageEvent, so when the player takes consecutive damage, the effect do not decrease
    public void ResetEffectTimer() 
    {
        effectTimerCounter = effectTimerMax;
    }


    //This is called in the player's OnHealEvent, so when the player is healed, the effect do not play, since it is meant to display damage only
    public void ResetEffect() 
    {
        effectSlider.value = slider.value;
    }
}

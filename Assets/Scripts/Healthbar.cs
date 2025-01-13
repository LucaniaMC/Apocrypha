using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;
    public Slider effectSlider;   //A second slider for smooth damage effect

    //Variables for damage effect, and effect that displays how much the player's health has decreased
    const float effectTimerMax = 1f;
    float effectTimer = 1f;
    const float effectSpeedMin = 20f;
    float effectSpeed;

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
        slider.value = health.health;

        //Damage effect block
        //The effect only start to decrease to the current health amount after a while, so the player can see how much damage they're taken
        effectTimer -= Time.deltaTime;

        //Variable effect speed based on the difference between the slider value and effect slider value, with a minimum speed
        effectSpeed = Mathf.Max(effectSpeedMin, (effectSlider.value - slider.value)  *2);
        
        //Smoothly move the damage effect bar towards the current health bar position
        if (effectTimer < 0 && effectSlider.value > health.health) 
        {
            effectSlider.value = (Mathf.MoveTowards(effectSlider.value, slider.value, effectSpeed * Time.deltaTime));
        }
    }


    //This is called in the player's OnDamageEvent, so when the player takes consecutive damage, the effect do not decrease
    public void ResetEffectTimer() 
    {
        effectTimer = effectTimerMax;
    }

    //This is called in the player's OnHealEvent, so when the player is healed, the effect do not play, since it is meant to display damage only
    public void ResetEffect() 
    {
        effectSlider.value = slider.value;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;

    public HealthSystem health;


    void Start()
    {
        slider.maxValue = health.maxHealth;
        slider.value = health.health;
    }

    
    void Update()
    {
        slider.value = health.health;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : Healthbar
{
    [SerializeField] private GameObject healthbarPosition;    // Where the health bar is centered at
    [SerializeField] private GameObject healthbarBase;        // Gameobject for the healthbar base that's toggled on and off

    protected override void Start()
    {
        base.Start();
        // Updates slider on start so it can be deactivated properly
        SliderUpdate();
    }


    protected override void Update()
    {
        base.Update();
        transform.position = Camera.main.WorldToScreenPoint(healthbarPosition.transform.position);
    }


    public override void SliderUpdate() 
    {
        base.SliderUpdate();
        // Deactivates the slide if the enemy is on max health
        if(health.health == health.maxHealth) 
        {
            healthbarBase.SetActive(false);
        }
        else
        {
            healthbarBase.SetActive(true);
        }
    }
}

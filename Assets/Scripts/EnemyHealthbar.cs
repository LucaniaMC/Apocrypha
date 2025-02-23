using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : Healthbar
{
    public GameObject healthbarPosition;
    public GameObject healthbarBase;

    protected override void Start()
    {
        base.Start();
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

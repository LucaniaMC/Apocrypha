using UnityEngine;

public class Wolf : GroundEnemy
{
    [Header("Visual Effect References")]
    public GameObject sprite;
    public GameObject spark;


    public override void OnDamage()
    {
        Instantiate(spark, new Vector3(sprite.transform.position.x , sprite.transform.position.y , 0), Quaternion.identity); 
        KnockBack();
    }

    public override void OnDeath()
    {
        Destroy(gameObject);
    }

    public override void Initialize()
    {
        defaultState = new WolfIdleState(this, this);
        base.Initialize();
    }
}

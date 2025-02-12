using UnityEngine;

public class Dummy : GroundEnemy
{
    [Header("Visual Effect References")]
    public GameObject sprite;
    public GameObject spark;


    public override void OnDamage()
    {
        sprite.GetComponent<Animator>().SetTrigger("Hurt");
        Instantiate(spark, new Vector3(sprite.transform.position.x , sprite.transform.position.y , 0), Quaternion.identity);
    }


    public override void OnDeath()
    {
        Destroy(gameObject);
    }
}

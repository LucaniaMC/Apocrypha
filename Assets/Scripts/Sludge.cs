using UnityEngine;

public class Sludge : MonoBehaviour
{

    public GameObject sprite;
    public GameObject spark;


    void OnTriggerEnter2D(Collider2D other) //Same as player attack, maybe they can share the same script?
    {
        if (other.gameObject.layer == 3) 
        {
            other.gameObject.GetComponent<HealthSystem>().Damage(10);
        }
    }

    public void onDamage()
    {
        Instantiate(spark, new Vector3(sprite.transform.position.x , sprite.transform.position.y , -2), Quaternion.identity); //-2 on Z axis to put the effect in front
    }


    public void Death()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;

public class DummyHurt : MonoBehaviour
{
    public GameObject sprite;
    public GameObject spark;


    public void onDamage()
    {
        sprite.GetComponent<Animator>().SetTrigger("Hurt");
        Instantiate(spark, new Vector3(sprite.transform.position.x , sprite.transform.position.y , -2), Quaternion.identity); //-2 on Z axis to put the effect in front
    }


    public void Death()
    {
        Destroy(gameObject);
    }
}

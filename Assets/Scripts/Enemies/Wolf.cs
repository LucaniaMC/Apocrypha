using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    public GameObject sprite;
    public GameObject spark;

    public void onDamage()
    {
        Instantiate(spark, new Vector3(sprite.transform.position.x , sprite.transform.position.y , 0), Quaternion.identity); 
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}

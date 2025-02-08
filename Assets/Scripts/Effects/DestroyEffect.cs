using UnityEngine;

//Attach this script to any effect object to have it self destruct after a given time

public class DestroyEffect : MonoBehaviour
{
    public float lifeTime = 0f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}

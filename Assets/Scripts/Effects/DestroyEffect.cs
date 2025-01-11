using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    public float lifeTime = 0f; //Lifetime of the particle effect

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}

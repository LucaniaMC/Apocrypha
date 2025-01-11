using UnityEngine;

public class Sludge : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) //Same as player attack, maybe they can share the same script?
    {
        if (other.gameObject.layer == 3) 
        {
            other.gameObject.GetComponent<HealthSystem>().Damage(10);
        }
    }
}

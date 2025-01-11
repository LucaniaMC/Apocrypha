using UnityEngine;

public class RandomizeEffect : MonoBehaviour
{
    public int randomCap;       //Number of random animations
    public Animator animator;   //Animator should have a Random parameter for animations


    void Start()
    {
        int randomNumber = Random.Range(1, randomCap + 1); //+1 since the upper bound of Random.Range is exclusive
        animator.SetInteger("Random", randomNumber);        
    }
}

using UnityEngine;

//Attach this script to any effect objects with multiple variations to randomly pick one of them

public class RandomizeEffect : MonoBehaviour
{
    public int randomCap;       //Number of random animations the object have
    public Animator animator;   //Animator should have an int Random parameter


    void Start()
    {
        int randomNumber = Random.Range(1, randomCap + 1); //+1 since the upper bound of Random.Range is exclusive
        animator.SetInteger("Random", randomNumber);        
    }
}

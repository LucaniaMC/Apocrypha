using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    //Attach this script to the main control objects, not the sprite objects

    [SerializeField] private Color flashColor = Color.white;    //Default flash color
    [SerializeField] private float flashTime = 0.2f;            //How long is the flash effect


    //Private fields
    private SpriteRenderer[] spriteRenderer;    //Array of all sprite renderers
    private Material[] material;                //Array of all materials used by sprite renderers

    private Coroutine flashRoutine;     //Coroutine for flashing effect


    void Awake()
    {
        //Get all sprites in children, useful for multiple sprites
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        //Set the length of the material array to the lebgth of the sprite renderer array
        material = new Material[spriteRenderer.Length];

        for (int i = 0; i < spriteRenderer.Length; i++) //Loop through each item in sprite renderer array
        {
            material[i] = spriteRenderer[i].material;           //Get the materials and store them in array
            material[i].SetColor("_FlashColor", flashColor);    //Set the flash color for each material
        } 
    }


    //Start the flash
    public void Flash()
    {
        // If the flashRoutine is not null, then it is currently running.
        if (flashRoutine != null)
        {
            // In this case, we should stop it first. Multiple FlashRoutines the same time would cause bugs.
            StopCoroutine(flashRoutine);
        }
        // Start the Coroutine, and store the reference for it.
        flashRoutine = StartCoroutine(FlashRoutine());
    }


    //Flash corontine
    private IEnumerator FlashRoutine()
    {
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < flashTime) 
        {
            //Timer
            elapsedTime += Time.deltaTime;

            //Lerp the current flash amount
            currentFlashAmount = Mathf.Lerp(1f, 0f, elapsedTime / flashTime);
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }


    //Sets the material's flash opacity to the Currentflashamount variable
    private void SetFlashAmount(float amount) 
    {
        for (int i = 0;  i < material.Length; i++) 
        {
            material[i].SetFloat("_FlashOpacity", amount);
        }
    }
}

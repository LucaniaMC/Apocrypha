using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    public GameObject landingParticle;
    public GameObject jumpParticle;
    public GameObject player;


    //Spawn landing particle
    //Currently unused because OnLandEvent() in PlayerMovement is bugged
    public void SpawnLandingParticle()
    {
        Instantiate(landingParticle, new Vector3(player.transform.position.x , player.transform.position.y , -1), Quaternion.identity);
    }


    //Spawn jump particle
    public void SpawnJumpParticle()
    {
        Instantiate(jumpParticle, new Vector3(player.transform.position.x , player.transform.position.y , -1), Quaternion.identity);
    }
}
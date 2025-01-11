using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    public GameObject landingParticle;
    public GameObject player;


    //Spawn landing particle
    public void SpawnLandingParticle()
    {
        Instantiate(landingParticle, new Vector3(player.transform.position.x , player.transform.position.y , -1), Quaternion.identity);
    }
}

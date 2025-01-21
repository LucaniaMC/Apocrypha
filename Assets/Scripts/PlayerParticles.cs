using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    public GameObject landingParticle;
    public GameObject jumpParticle;
    public GameObject player;
    public ParticleSystem dashParticle;

    void Start() 
    {
        dashParticle.Stop();
    }

    public void SpawnLandingParticle()
    {
        Instantiate(landingParticle, new Vector3(player.transform.position.x , player.transform.position.y , -1), Quaternion.identity);
    }


    public void SpawnJumpParticle()
    {
        Instantiate(jumpParticle, new Vector3(player.transform.position.x , player.transform.position.y , -1), Quaternion.identity);
    }


    public void StartDashParticle() 
    {
        dashParticle.Play();
    }


    public void StopDashParticle() 
    {
        dashParticle.Stop();
    }
}

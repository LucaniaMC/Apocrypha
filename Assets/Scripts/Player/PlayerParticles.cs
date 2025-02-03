using UnityEngine;

//This is a partial class for Player.cs containing functions that handle player particles
//Only Player.cs needs to be attatched to an object

public partial class Player
{
    public void SpawnLandingParticle()
    {
        Instantiate(landingParticle, new Vector3(this.transform.position.x , this.transform.position.y , 0), Quaternion.identity);
    }


    public void SpawnJumpParticle()
    {
        Instantiate(jumpParticle, new Vector3(this.transform.position.x , this.transform.position.y , 0), Quaternion.identity);
    }


    //Set dash particles, true to start particle, false to stop particle
    public void SetDashParticle(bool isActive) 
    {
        if (isActive) 
        {
            dashParticle.Play();
        }
        else 
        {
            dashParticle.Stop();
        } 
    }

    public void SetWallParticle(bool isActive) 
    {
        if (isActive) 
        {
            wallParticle.Play();
        }
        else 
        {
            wallParticle.Stop();
        } 
    }
}

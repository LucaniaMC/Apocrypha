using UnityEngine;

[ExecuteInEditMode]
public class Parallax : MonoBehaviour
{
    public float percentX;      //How much the object moves on X axis. Positive values for background objects, negative values for foreground objects
    public float percentY;      //How much the object moves on Y axis.
    [SerializeField] private Camera cam;    //Reference camera
    Vector3 camPrev;    //Camera origin position


    // Start is called before the first frame update
    void Start()
    {
        camPrev = cam.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 camPos = cam.transform.position;
        float deltaX = camPos.x - camPrev.x;
        float deltaY = camPos.y - camPrev.y;

        float adjustX = deltaX * percentX;
        float adjustY = deltaY * percentY;

        transform.position = transform.position + new Vector3(adjustX, adjustY, 0);

        camPrev = camPos;
    }
}
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public bool OnPlace
    {
        get
        {
            return transform.localScale.x > 0 && transform.localScale.y > 0;
        }
        
        set
        {
            if(value == true)
            {
                transform.localScale = new Vector3(1,1,1);
                GetComponentInChildren<ParticleSystem>().Stop();
                //particles off
            }
            else
            {
                transform.localScale = Vector3.zero;
                GetComponentInChildren<ParticleSystem>().Play();
                //particles on;
            }
        }
    }
}

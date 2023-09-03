using UnityEngine;

public class ZombieRagdollBehaviour : MonoBehaviour, ICrowdUpdate
{
    [SerializeField] private Transform hips; // For add hips, need to fix  to incode
    private const float BodyTimer = 3;
    private ZombieHand[] zombieHands;
    private Rigidbody[] bonesRb;
    private Collider[] bonesColliders;
    private Rigidbody hipsRb;
    private ZombieBrain zc;
    private float ragdollTime;
    private bool onPlayerCatch;
    private int _rbCounter;
    private Vector3 rbVelocity;
    private ZombiePartition zp;

    public void AddExplosionForce(float explForce, Vector3 explPosition, float explRadius)
    {
        foreach(var rb in bonesRb)
        {
            rb.AddExplosionForce(explForce, explPosition, explRadius);
        }
    }

    public void AddImpulse(Vector3 impulse)
    {
        foreach(var rb in bonesRb)
        {
            rb.AddForce(impulse, ForceMode.VelocityChange);
        }
    }

    public void CatchPlayer(Rigidbody player)
    {
        if(zp.CanCatch)
        {
            onPlayerCatch = true;
            UpdateBones(true);   
            foreach(var hand in zombieHands)
            {
                hand.CatchPlayer(player, this);
                
            }
        }
    }

    public void LosePlayer()
    {
        onPlayerCatch = false;
        ragdollTime = Time.time;
    }

    private void OnEnable()
    {
        if(!zc)
        {
            zp = GetComponent<ZombiePartition>();
            zc = GetComponent<ZombieBrain>();
            hipsRb = hips.GetComponent<Rigidbody>();
            bonesRb = GetComponentsInChildren<Rigidbody>();
            bonesColliders = hips.GetComponentsInChildren<Collider>();
            zombieHands = GetComponentsInChildren<ZombieHand>();
        }
        ragdollTime = Time.time;
        UpdateBones(true);
    }


    private void OnDisable()
    {
        UpdateBones(false);
        StopAllCoroutines();
    }

    public void UpdateBones(bool showBones)
    {
        hips.localPosition = Vector3.zero;
        foreach(var collider in bonesColliders)
        {
            collider.enabled = showBones;
        }
        foreach(var rb in bonesRb)
        {
            if(!rb.isKinematic)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = !showBones;
            rb.useGravity = showBones;
        }
    }



    public void CrowdUpdate(Vector3 playerPos)
    {
        if((hipsRb.velocity.x < 1 && hipsRb.velocity.x > -1) && (hipsRb.velocity.y < 1 && hipsRb.velocity.y > -1) && (hipsRb.velocity.z < 1 && hipsRb.velocity.z > -1))
        {
            if(Time.time > ragdollTime + BodyTimer && !onPlayerCatch)
            {
                zc.RagdollCalmedOut();
            }
        }
        
        rbVelocity = bonesRb[RbCounter].velocity;
        rbVelocity.z *= Time.fixedDeltaTime;
        bonesRb[RbCounter].velocity = rbVelocity;
        RbCounter++;
        
    }

    private int RbCounter
    {
        get
        {
            return _rbCounter;
        }
        set
        {
            _rbCounter = value;
            if(_rbCounter >= bonesRb.Length)
            {
                _rbCounter = 0;
            }
        }
    }

}

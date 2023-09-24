using System.Collections;
using UnityEngine;

namespace Crowd.Zombie
{
public class ZombieBrain : ExtendedPhysics.Damage.Receiver.ExplosionReceiverBehaviour
{
    [SerializeField]
    private string CURRENT_STATE; //To show in Unity UI

    [SerializeField]
    private float DistanceToPlayer; // To use & show in Unity UI

    [SerializeField]
    private float hp; // To use & show in Unity UI
    private const float MAX_HP = 100;
    private const float PARTITION_ZONE = 30;
    private const float BodyTimer = 3;
    private ZombieManager zm;
    private ZombieAIBehaviour zombieAi;
    private ZombieRagdollBehaviour zombieRagdoll;
    private DamageCalculator dc;
    private ZombiePartition zp;
    private bool _ragdollMode;
    private ZombieState _zState;
    private ICrowdUpdate currentBehaviour;
    private Collider collider;
    private bool isAttacking;

    public void Initialize(Vector3 position, Transform parent, ZombieManager zombManager)
    {
        if (zm == null)
        {
            zm = zombManager;
            InitializeFields();
        }

        SetStartSettings(position, parent);
    }

    private void InitializeFields()
    {
        zombieAi = GetComponent<ZombieAIBehaviour>();
        zp = GetComponent<ZombiePartition>();
        zombieRagdoll = GetComponent<ZombieRagdollBehaviour>();
        collider = GetComponent<Collider>();
        dc = gameObject.AddComponent<DamageCalculator>();
    }

    private void SetStartSettings(Vector3 position, Transform parent)
    {
        hp = MAX_HP;
        transform.position = position;
        transform.SetParent(parent);

        zp.SetToZero();
        zombieRagdoll.UpdateBones(false);

        collider.enabled = true;
        ZState = ZombieState.Ai;
        zombieAi.enabled = true;
        zombieRagdoll.enabled = false;
    }

    public void UpdatePlayerPosition(Vector3 playerPosition, Vector3 playerProjection)
    {
        if (currentBehaviour != null && ZState != ZombieState.Dead)
        {
            currentBehaviour.CrowdUpdate(playerProjection);
        }
        if (transform.position.y < -1)
        {
            zm.HideZombie(gameObject);
        }

        DistanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        if (DistanceToPlayer < 10 && ZState == ZombieState.Ai)
        {
            ZomibePlayerJump(playerPosition);
        }
    }

    public void ZombieFall()
    {
        AddZombieForce((transform.forward + transform.up) * 8);
    }

    public void RagdollCalmedOut()
    {
        if (ZState != ZombieState.Dead)
        {
            ZState = ZombieState.Ai;
        }
    }

    public override void AddExplosionForce(
        float explosionForce,
        Vector3 explosionPosition,
        float explosionRadius,
        float upwardsModifier,
        ForceMode forceMode
    )
    {
        float damage = dc.CalculateDamage(explosionPosition, explosionRadius);
        ZState = StillAlive(damage) ? ZombieState.Ragdoll : ZombieState.Dead;
        base.AddExplosionForce(
            explosionForce,
            explosionPosition,
            explosionRadius,
            upwardsModifier,
            forceMode
        );
    }

    public void AddZombieForce(Vector3 force)
    {
        ZState = ZombieState.Ragdoll;
        zombieRagdoll.AddImpulse(force);
    }

    private ZombieState ZState
    {
        get { return _zState; }
        set
        {
            if (_zState != value)
            {
                _zState = value;
                ChangeState();
            }
        }
    }

    private void ChangeState()
    {
        switch (ZState)
        {
            case ZombieState.Ai:
                RagdollMode = false;
                isAttacking = false;
                currentBehaviour = zombieAi;
                break;
            case ZombieState.Ragdoll:
                RagdollMode = true;
                currentBehaviour = zombieRagdoll;
                break;
            case ZombieState.Dead:
                currentBehaviour = null;
                RagdollMode = true;
                zm.DeadZombie(gameObject);
                if (gameObject.activeSelf)
                {
                    StartCoroutine(ShowZombieDeath());
                }
                break;
        }
    }

    IEnumerator ShowZombieDeath()
    {
        yield return new WaitForSeconds(BodyTimer);
        zm.HideZombie(gameObject);
    }

    private void ZomibePlayerJump(Vector3 playerPosition)
    {
        isAttacking = true;
        AddZombieForce((playerPosition - transform.position) * 2);
    }

    private enum ZombieState
    {
        Ragdoll,
        Ai,
        Dead
    }

    private bool RagdollMode
    {
        get { return _ragdollMode; }
        set
        {
            if (_ragdollMode != value)
            {
                _ragdollMode = value;
                zombieRagdoll.enabled = _ragdollMode;
                zombieAi.enabled = !value;
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && isAttacking)
        {
            zombieRagdoll.CatchPlayer(other.gameObject.GetComponent<Rigidbody>());
        }
        else if (!other.gameObject.CompareTag("Zombie"))
        {
            float damage = dc.CalculateDamage(other.impulse);
            if (!StillAlive(damage))
            {
                ZState = ZombieState.Dead;
            }
            else if (damage > 30)
            {
                ZState = ZombieState.Ragdoll;
            }
        }
    }

    private bool StillAlive(float damage)
    {
        CalculatePartition(damage);
        hp -= damage;
        return hp > 0;
    }

    private void CalculatePartition(float damage)
    {
        float damagePercent = damage * 100 / MAX_HP;
        if (damagePercent > PARTITION_ZONE)
        {
            zp.RandomPartition();
        }
    }

    private void Update()
    {
        CURRENT_STATE = ZState.ToString(); // To show in Unity UI
    }
}
}
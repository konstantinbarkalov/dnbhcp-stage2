using System.Collections;
using UnityEngine;

namespace Crowd.Zombie
{
    // TODO: rewrite this shit from scratch
public class ZombieBrain : ExtendedPhysics.Damage.Receiver.ExplosionReceiverBehaviour
{

    [SerializeField]
    private float hp; // To use & show in Unity UI
    private const float maxHp = 100;
    private const float partitionDamageThershold = 30;
    private const float bodyTimer = 3;
    private ZombieManager zm;
    private ZombieAIBehaviour zombieAi;
    private ZombieRagdollBehaviour zombieRagdoll;
    private DamageCalculator damageCalculator;
    private ZombiePartition partition;
    private bool _ragdollMode;
    private ZombieState _state;
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
        partition = GetComponent<ZombiePartition>();
        zombieRagdoll = GetComponent<ZombieRagdollBehaviour>();
        collider = GetComponent<Collider>();
        damageCalculator = gameObject.AddComponent<DamageCalculator>();
    }

    private void SetStartSettings(Vector3 position, Transform parent)
    {
        hp = maxHp;
        transform.position = position;
        transform.SetParent(parent);

        partition.SetToZero();
        zombieRagdoll.UpdateBones(false);

        collider.enabled = true;
        State = ZombieState.Ai;
        zombieAi.enabled = true;
        zombieRagdoll.enabled = false;
    }

    public void UpdatePlayerPosition(Vector3 playerPosition, Vector3 playerProjection)
    {
        if (currentBehaviour != null && State != ZombieState.Dead)
        {
            currentBehaviour.CrowdUpdate(playerProjection);
        }
        if (transform.position.y < -1)
        {
            zm.HideZombie(gameObject);
        }

        var distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        if (distanceToPlayer < 10 && State == ZombieState.Ai)
        {
            ZomibiePlayerJump(playerPosition);
        }
    }

    public void ZombieFall()
    {
        AddZombieForce((transform.forward + transform.up) * 8);
    }

    public void RagdollCalmedOut()
    {
        if (State != ZombieState.Dead)
        {
            State = ZombieState.Ai;
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
        float mass = 100; // TODO: get from rigidbody
        float damage = damageCalculator.CalculateDamage(mass, explosionForce, explosionPosition, explosionRadius, forceMode);
        State = StillAlive(damage) ? ZombieState.Ragdoll : ZombieState.Dead;
        base.AddExplosionForce(
            explosionForce,
            explosionPosition,
            explosionRadius,
            upwardsModifier,
            forceMode
        );
    }

    public void AddZombieForce(Vector3 force) // WTF: why we need this?
    {
        State = ZombieState.Ragdoll;
        zombieRagdoll.AddImpulse(force);
    }

    private ZombieState State
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                _state = value;
                ChangeState();
            }
        }
    }

    private void ChangeState()
    {
        switch (State)
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
        yield return new WaitForSeconds(bodyTimer);
        zm.HideZombie(gameObject);
    }

    private void ZomibiePlayerJump(Vector3 playerPosition)
    {
        isAttacking = true;
        AddZombieForce((playerPosition - transform.position) * 3);
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
            float damage = damageCalculator.CalculateDamage(other.impulse);
            if (!StillAlive(damage))
            {
                State = ZombieState.Dead;
            }
            else if (damage > 30)
            {
                State = ZombieState.Ragdoll;
            }
        }
    }
    private bool StillAlive(float damage) // WFT: Silly naming
    {
        CalculatePartition(damage);
        hp -= damage;
        return hp > 0;
    }

    private void CalculatePartition(float damage) // WFT: Silly naming
    {
        float damagePercent = damage * 100 / maxHp;
        if (damagePercent > partitionDamageThershold)
        {
            partition.RandomPartition();
        }
    }

}
}
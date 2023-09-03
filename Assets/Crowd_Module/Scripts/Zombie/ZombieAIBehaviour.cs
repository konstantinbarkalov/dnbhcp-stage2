using UnityEngine;
using UnityEngine.AI;

public class ZombieAIBehaviour : MonoBehaviour, ICrowdUpdate
{
    [SerializeField] private string currentStateString; //To show in Unity UI
    private NavMeshAgent agent;
    private Animator animator;
    private Collider collider;
    private ZombieState _zombieState;
    private ZombieState currentWalkState;
    private ZombieBrain zb;
    private bool updatePlayerPosition;
    private ZombiePartition zp;
    
    private enum ZombieState
    {
        Walk, // 0
        Crawl, // 1
        Climb, // 2
    }
    public void CrowdUpdate(Vector3 position)
    {
        if(agent.enabled && updatePlayerPosition)
        {
            agent.SetDestination(position);
        }
    }

    private void OnEnable()
    {
        if(!agent)
        {
            Initial();
        }
        agent.enabled = true;
        animator.enabled = true;
        collider.enabled = true;
        updatePlayerPosition = true;
        animator.SetFloat("zombSpeed", agent.speed);
        UpdateState();
    }

    private void OnDisable()
    {
        agent.enabled = false;
        animator.enabled = false;
        updatePlayerPosition = false;
    }
    
    private ZombieState ZombState
    {
        get
        {
            return _zombieState;
        }
        set
        {
            if(_zombieState != value)
            {
                _zombieState = value;
                UpdateState();
            }
        }
    }

    private void UpdateState()
    {
        currentWalkState = zp.CanWalk? ZombieState.Walk : ZombieState.Crawl;
        animator.SetInteger("State", (int)ZombState);
    }

    private void Initial()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
        zb = GetComponentInParent<ZombieBrain>();
        zp = GetComponent<ZombiePartition>();
        currentWalkState = ZombieState.Walk;
        SetRandomSpeed();
    }

    private void SetRandomSpeed()
    {
        float speed = Random.Range(5f, 10f);
        agent.speed = speed;
        animator.SetFloat("zombSpeed", speed);
    }

    private void FixedUpdate(){
        currentStateString = ZombState.ToString(); ///To show in Unity UI
        if(agent.isOnOffMeshLink && agent.currentOffMeshLinkData.offMeshLink)
        {
            switch(agent.currentOffMeshLinkData.offMeshLink.area)
            {
                case 3:
                    ZombState = ZombieState.Climb;
                    break;
                case 4:
                    Fall();
                    break;
            }
        }
        else
        {
            ZombState = currentWalkState;
        }
    }

    private void Fall()
    {
        zb.ZombieFall();
    }


}

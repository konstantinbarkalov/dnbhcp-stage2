using System.Collections;
using UnityEngine;
using TMPro;
public class ZombieManager : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab; // To add prefab, maybe change to Resources later;
    [SerializeField] private TMP_InputField input; // To add inputField, for changing maxZombie at the screen
    private int zombiesCount; 
    private ZombieBrain[] zombieCrowd;
    private ObjectPool zombiePool;
    private SpawnPoint[] spawnPoints;
    private Transform player, playerProjection;
    private Transform spawnPoint;
    private const int MaxRagDolls = 30;
    private int _RagdollCounter;

    private int RagdollCounter
    {
        get
        {
            return _RagdollCounter;
        }

        set
        {
            _RagdollCounter = value;
            if(_RagdollCounter < 0)
            {
                _RagdollCounter = 0;
            }    
        }
    }
    private void Update()
    {
        zombiesCount = int.Parse(input.text);
    }
    private void Start()
    {
        RagdollCounter = 0;
        zombiesCount = int.Parse(input.text);
        playerProjection = GameObject.FindWithTag("PlayerProjection").transform;
        player = GameObject.FindWithTag("Player").transform;
        spawnPoints = GetComponentsInChildren<SpawnPoint>();
        zombiePool = new ObjectPool(zombiePrefab);
        StartCoroutine(SpawnZombies());
        StartCoroutine(WalkZombes());
    }
    IEnumerator SpawnZombies()
    {
        while(true)
        {
            zombieCrowd = GetComponentsInChildren<ZombieBrain>();
            if(zombieCrowd.Length < zombiesCount)
            {
                foreach(var spoint in spawnPoints)
                {
                    if(!spoint.Visible)
                    {
                        spawnPoint = spoint.transform;
                        break;
                    }
                }

                foreach(var spoint in spawnPoints)
                {
                    if(Vector3.Distance(spoint.transform.position, player.position) < Vector3.Distance(spawnPoint.position, player.position) && !spoint.Visible)
                    {
                        spawnPoint = spoint.transform;
                    }
                }
                zombiePool.GetObjectPoolObject().GetComponent<ZombieBrain>()
                    .Initial(spawnPoint.position, transform, this);
                RagdollCounter--;
                yield return null;
            }
            yield return null;
        }
        
    }
    IEnumerator WalkZombes()
    {
        while(true)
        {
            zombieCrowd = GetComponentsInChildren<ZombieBrain>();
            foreach(var zombe in zombieCrowd)
            {
                if(zombe && zombe.gameObject.activeSelf)
                {
                    zombe.UpdatePlayerPosition(player.position, playerProjection.position);
                }
                
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
            yield return null;

        }
    }
    public void HideZombie(GameObject zombe)
    {
        zombiePool.AddToPool(zombe);
    }
    public void DeadZombie(GameObject zombe)
    {
        if(RagdollCounter < MaxRagDolls)
        {
            RagdollCounter++;
        }
        else
        {
            HideZombie(zombe);
        }
    }
    public void Clear()
    {
        zombieCrowd = GetComponentsInChildren<ZombieBrain>();
        foreach(var zombe in zombieCrowd)
        {
            HideZombie(zombe.gameObject);
        }

    }
}

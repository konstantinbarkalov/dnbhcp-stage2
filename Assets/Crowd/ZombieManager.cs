using System.Collections;
using UnityEngine;
using TMPro;
namespace Crowd
{

public class ZombieManager : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab; // To add prefab, maybe change to Resources later;
    private int zombiesCount; 
    private Zombie.ZombieBrain[] zombieBrains;
    private Pool zombiePool;
    private SpawnPoint[] spawnPoints;
    private Transform player, playerProjection;
    private Transform spawnPoint;
    private const int MaxRagDolls = 30;
    private int _RagdollCounter;

    private int ragdollCount
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
    private void Start()
    {
        ragdollCount = 0;
        zombiesCount = PlayerPrefs.GetInt("zombies-count", 100);
        playerProjection = GameObject.FindWithTag("PlayerProjection").transform;
        player = GameObject.FindWithTag("Player").transform;
        spawnPoints = GetComponentsInChildren<SpawnPoint>();
        zombiePool = new Pool(zombiePrefab);
        StartCoroutine(SpawnZombies());
        StartCoroutine(WalkZombies());
    }
    IEnumerator SpawnZombies()
    {
        while(true)
        {
            zombieBrains = GetComponentsInChildren<Zombie.ZombieBrain>();
            if(zombieBrains.Length < zombiesCount)
            {
                int spawnPointIdx = Mathf.FloorToInt(Random.value * spawnPoints.Length); 
                spawnPoint = spawnPoints[spawnPointIdx].transform;
                zombiePool.Get().GetComponent<Zombie.ZombieBrain>()
                    .Initialize(spawnPoint.position, transform, this);
                ragdollCount--;
                yield return null;
            }
            yield return null;
        }
        
    }
    IEnumerator WalkZombies()
    {
        while(true)
        {
            zombieBrains = GetComponentsInChildren<Zombie.ZombieBrain>();
            foreach(var zombie in zombieBrains)
            {
                if(zombie && zombie.gameObject.activeSelf)
                {
                    zombie.UpdatePlayerPosition(player.position, playerProjection.position);
                }
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
    public void HideZombie(GameObject zombie)
    {
        zombiePool.Put(zombie);
    }
    public void DeadZombie(GameObject zombie)
    {
        MetaManager.level.scoreManager.score++;
        if(ragdollCount < MaxRagDolls)
        {
            ragdollCount++;
        }
        else
        {
            HideZombie(zombie);
        }
    }
    public void Clear()
    {
        zombieBrains = GetComponentsInChildren<Zombie.ZombieBrain>();
        foreach(var zombie in zombieBrains)
        {
            HideZombie(zombie.gameObject);
        }

    }
}
}
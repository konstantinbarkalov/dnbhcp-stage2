using System.Collections;
using UnityEngine;
using TMPro;
namespace Crowd
{

public class ZombieManager : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab; // To add prefab, maybe change to Resources later;
    [SerializeField] private TMP_InputField input; // To add inputField, for changing maxZombie at the screen
    private int zombiesCount; 
    private Zombie.ZombieBrain[] zombieBrains;
    private Pool zombiePool;
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
                RagdollCounter--;
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
                
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
            yield return null;

        }
    }
    public void HideZombie(GameObject zombie)
    {
        zombiePool.Put(zombie);
    }
    public void DeadZombie(GameObject zombie)
    {
        MetaManager.level.scoreManager.score++;
        if(RagdollCounter < MaxRagDolls)
        {
            RagdollCounter++;
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
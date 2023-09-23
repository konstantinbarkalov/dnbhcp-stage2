using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBayBehaviour : MonoBehaviour
{
  public HelicopterBehaivour helicopter;
  public BombBehaviour[] bombPrefabs;
  
  public float shootPower = 10000;
  public Transform bombSpawnAnchor;
  
  
  void Start()
  {

  }

  // Update is called once per frame
  void FixedUpdate()
  {
    BombtrackEntity newBomb = MetaManager.level.hypertrackManager.GetNewBomb();
    if (newBomb != null) {
      int bombPrefabIdx = ((newBomb.type - 1) + bombPrefabs.Length) % bombPrefabs.Length;
      DropBomb(newBomb.explodeTime-newBomb.dropTime, bombPrefabIdx);
    }
  }
  void DropBomb(float bombExplodeTimeout, int bombPrefabIdx)
  {
    BombBehaviour bombPrefab = bombPrefabs[bombPrefabIdx];
    BombBehaviour newBombInstance = Instantiate<BombBehaviour>(bombPrefab, bombSpawnAnchor.position, bombSpawnAnchor.rotation);
    newBombInstance.exlpodeTime = Time.fixedTime + bombExplodeTimeout;
    Vector3 bombInertiaVelocity = helicopter.rigidBody.GetPointVelocity(bombSpawnAnchor.position);
    Rigidbody bombRigidbody = newBombInstance.GetComponent<Rigidbody>();
    bombRigidbody.AddForce(bombInertiaVelocity, ForceMode.VelocityChange);
    
    Vector3 shootDirection = bombSpawnAnchor.up * -1;
    Vector3 shootForce = shootDirection * shootPower;
    bombRigidbody.AddForce(shootForce, ForceMode.Force);
  }
}

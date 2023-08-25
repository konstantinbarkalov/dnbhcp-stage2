using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(BalloonBehaviour))]
public class BalloonTorchAnimatorBehaviour : MonoBehaviour
{
    public int shift = 0;
    public int mod = 3;
    private BombtrackEntity bomb;
    private int bombIdx;
    private BalloonBehaviour balloon;
    void Start() {
        balloon = GetComponent<BalloonBehaviour>();
    }
    void FixedUpdate()
    {
        BombtrackEntity newBomb = MetaManagerBehaviour.metaManager.hypertrackManager.GetNewBomb();
        if (newBomb != null)
        {
            int modBombIdx = (bombIdx + shift) % mod;
            if (modBombIdx == 0) 
            {
                bomb = newBomb;
            }
            bombIdx++;
        }
    }
    
    void Update()
    {
        bool hasBomb = (bomb != null);
        if (hasBomb) {
            bool isExploded = bomb.explodeTime <= MetaManagerBehaviour.metaManager.hypertrackManager.source.time;
            bool isEnded = bomb.explodeTime + 1 <= MetaManagerBehaviour.metaManager.hypertrackManager.source.time;
            balloon.isFullThrottleEnabled = (isExploded && !isEnded);
            balloon.isTorchEnabled = isExploded;
        } else {
            balloon.isFullThrottleEnabled = false;
            //balloon.isTorchEnabled = true;
        }
        //balloon.isStartFly = MetaManagerBehaviour.metaManager.hypertrackManager.source.time > 64 + 8 + 2;
        balloon.isStartFly = MetaManagerBehaviour.metaManager.hypertrackManager.source.time > 64; 
        
    }
}

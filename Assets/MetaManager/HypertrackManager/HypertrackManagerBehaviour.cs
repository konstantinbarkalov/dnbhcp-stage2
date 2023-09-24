using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypertrackManagerBehaviour : AbstractLevelManagerBehaviour
{
  public AudioSource source;
  public TextAsset bombtrackJsonFile;


  private BombtrackEntity newBomb = null;
  private Bombtrack bombtrackJson;
  private int bombtrackCursor = -1;
  public BombtrackEntity GetNewBomb()
  {
    return newBomb;
  }
  public float GetBeat()
  {
    float bpm = 180;
    float beat = MetaManager.level.hypertrackManager.source.time / 60 * bpm;
    return beat;
  }

  void Start()
  {
    newBomb = null;
    bombtrackCursor = -1;
    bombtrackJson = JsonUtility.FromJson<Bombtrack>(bombtrackJsonFile.text);
    source.Play();
    source.time = 7;
    //source.time = 64;
  }

  void FixedUpdate()
  {
    newBomb = GetNewBomb__FixedUpdate();
  }
  private BombtrackEntity GetNewBomb__FixedUpdate()
  {
    float soundtrackTime = source.time;
    BombtrackEntity[] bombs = bombtrackJson.bombs;
    int newBombtrackCursor = bombtrackCursor;
    for (int bombIdx = bombtrackCursor + 1; bombIdx < bombs.Length; bombIdx++)
    {
      BombtrackEntity bomb = bombs[bombIdx];
      if (bomb.dropTime > soundtrackTime)
      {
        break;
      }
      else
      {
        newBombtrackCursor = bombIdx;
      }
    }
    if (newBombtrackCursor != bombtrackCursor)
    {
      BombtrackEntity lastBomb = bombs[newBombtrackCursor];
      bombtrackCursor = newBombtrackCursor;
      return lastBomb;
    }
    else
    {
      return null;
    }
  }
}

[System.Serializable]
public class Bombtrack
{
  public BombtrackEntity[] bombs;
}


[System.Serializable]
public class BombtrackEntity
{
  public int type;
  public float dropTime;
  public float explodeTime;
}

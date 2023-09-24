using System.Collections.Generic;
using UnityEngine;
namespace Crowd.Zombie
{
public class ZombiePartition : MonoBehaviour
{
    // 0 - leftLeg
    // 1 - rightLeg
    // 2 - LeftArm
    // 3 - Head
    // 4 - rightArm
    // Unity ierarchy guarantee
    private BodyPart[] bodyParts;
    private List<BodyPart> partsOnPlace = new List<BodyPart>();
    public bool CanCatch
    {
        get { return bodyParts[2].OnPlace && bodyParts[4].OnPlace; }
    }

    public bool CanWalk
    {
        get { return bodyParts[0].OnPlace && bodyParts[1].OnPlace; }
    }

    public bool HasHead
    {
        get { return bodyParts[3].OnPlace; }
    }

    public void SetToZero()
    {
        bodyParts = GetComponentsInChildren<BodyPart>();
        partsOnPlace.Clear();
        partsOnPlace.AddRange(bodyParts);
        foreach (var part in bodyParts)
        {
            part.OnPlace = true;
        }
    }

    public void RandomPartition()
    {
        //int part = Random.Range(0, partsOnPlace.Count);
        //partsOnPlace[part].OnPlace = false;
        //partsOnPlace.RemoveAt(part);
    }
}
}
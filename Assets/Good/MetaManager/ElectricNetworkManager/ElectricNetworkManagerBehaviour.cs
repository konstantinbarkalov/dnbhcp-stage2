using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum LaneState
{
    On,
    Off,
    Fluctuate
}
public class ElectricNetworkManagerBehaviour : MonoBehaviour
{
    private float[] lanes = new float[8];

    public float GetLaneRatio(int laneIdx)
    {
        return lanes[laneIdx];
    }
    public bool GetLaneState(int laneIdx)
    {
        return GetLaneRatio(laneIdx) > 0.5;
    }

    void FixedUpdate()
    {
        UpdateLanes__FixedUpdate();
    }

    private void UpdateLanes__FixedUpdate()
    {

        float beat = MetaManagerBehaviour.metaManager.hypertrackManager.GetBeat();
        float nightElectricBeginBeat = 64 + 16;
        float blackoutBeginBeat = 128 + 64;
        float blackoutEndBeat = blackoutBeginBeat + 32;
        float nightElectricEndBeat = blackoutEndBeat + 64;
        
        float fluctuationBeatDuration = 4;
        float fullOffBeatDuration = 2;
        
        
        for (int laneIdx = 0; laneIdx < lanes.Length; laneIdx++)
        {
            // Part 1
            // beat to laneState
            float beatShift = laneIdx % 4;
            bool isNightLight = laneIdx < 4;
            LaneState laneState;
            if (beat >= nightElectricEndBeat - beatShift)
            {
                laneState = isNightLight ? LaneState.Off : LaneState.On;
            }
            else if (beat >= nightElectricEndBeat - fluctuationBeatDuration - beatShift)
            {
                laneState = isNightLight ? LaneState.Fluctuate : LaneState.On;
            }
            else if (beat >= blackoutEndBeat) // no beatshift
            {
                laneState = LaneState.On;
            }
            else if (beat >= blackoutEndBeat - fullOffBeatDuration) // no beatshift
            {
                laneState = LaneState.Off;
            }
            else if (beat >= blackoutEndBeat - fluctuationBeatDuration - beatShift)
            {
                laneState = LaneState.Fluctuate;
            }
            else if (beat >= blackoutBeginBeat - beatShift)
            {
                laneState = LaneState.Off;
            }
            else if (beat >= blackoutBeginBeat - fluctuationBeatDuration - beatShift)
            {
                laneState = LaneState.Fluctuate;
            }
            else if (beat >= nightElectricBeginBeat - beatShift)
            {
                laneState = LaneState.On;
            }
            else if (beat >= nightElectricBeginBeat - fluctuationBeatDuration - beatShift)
            {
                laneState = isNightLight ? LaneState.Fluctuate : LaneState.On;
            }
            else
            {
                laneState = isNightLight ? LaneState.Off : LaneState.On;
            };
            
            
            // Part 2
            // laneState to actual ratio value 
            if (laneState == LaneState.On)
            {
                lanes[laneIdx] = 1;
            }
            else if (laneState == LaneState.Off)
            {
                lanes[laneIdx] = 0;
            }
            else if (laneState == LaneState.Fluctuate)
            {
                float chanceNotToFloatWhileFrame = 0.5f;
                if (Random.value > chanceNotToFloatWhileFrame)
                {
                    lanes[laneIdx] = Random.value;
                }
                float unfadePerSec = 0.5f;
                float unfadePerFrame = Mathf.Pow(unfadePerSec, Time.fixedDeltaTime);
                lanes[laneIdx] *= unfadePerFrame;
            } 
            else 
            {
                throw new System.NotImplementedException();
            }
        }
    }
}

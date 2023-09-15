using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhoreState {
    Go,
    Stop,
    Warn,
    WarnBeforeStop,
    WarnBeforeGo,
    Off,
    Full,
    Alarm,
    Error,
    Disco
}
public class PhoreBehaviour : MonoBehaviour
{
    public PhoreState state;
    public Renderer g;
    public Renderer y;
    public Renderer r;
    public new Light light;
    public float lightIntensity;
    private Material gMaterial;
    private Material yMaterial;
    private Material rMaterial;

    private void Start()
    {
        gMaterial = g.material;
        yMaterial = y.material;
        rMaterial = r.material;
    }

    private void Update()
    {
        ResolveState__Update();
        ResolveLight__Update();
    }
    private void ResolveLight__Update()
    {
        Color compositeColor = Color.black;
        float intensityFactor = 0;
        if (g.enabled) {
            compositeColor.g += 0.75f;
            intensityFactor++;
        }
        if (y.enabled) {
            compositeColor.r += 1;
            compositeColor.g += 0.75f;
            intensityFactor++;
        }
        if (r.enabled) {
            compositeColor.r += 1;
            intensityFactor++;
        }
        light.color = compositeColor / intensityFactor;
        light.intensity = intensityFactor * lightIntensity;
        light.enabled = intensityFactor > 0;
    }
    private void ResolveState__Update() {
        float beat = MetaManagerBehaviour.instance.hypertrackManager.GetBeat(); 
        bool blink = (beat * 2) % 2 < 1;
        switch (state)
        {
            case PhoreState.Go:
                g.enabled = true;
                y.enabled = false;
                r.enabled = false;
                break; 
            case PhoreState.Stop:
                g.enabled = false;
                y.enabled = false;
                r.enabled = true;
                break; 
            case PhoreState.Warn:
            case PhoreState.WarnBeforeStop:
                g.enabled = false;
                y.enabled = blink;
                r.enabled = false;
                break; 
            case PhoreState.WarnBeforeGo:
                g.enabled = false;
                y.enabled = true;
                r.enabled = true;
                break; 
            case PhoreState.Off:
                g.enabled = false;
                y.enabled = false;
                r.enabled = false;
                break; 
            case PhoreState.Full:
                g.enabled = true;
                y.enabled = true;
                r.enabled = true;
                break; 
            case PhoreState.Alarm:
                g.enabled = blink;
                y.enabled = blink;
                r.enabled = blink;
                break; 
            case PhoreState.Error:
                g.enabled = Random.value < 0.5;
                y.enabled = Random.value < 0.5;
                r.enabled = Random.value < 0.5;
                break; 
            case PhoreState.Disco:
                g.enabled = (beat + 1) % 4 < 1;
                y.enabled = (beat + 2) % 4 < 1;
                r.enabled = (beat + 3) % 4 < 1;
                break; 
            default:
            throw new System.NotImplementedException();
        }
    }
}

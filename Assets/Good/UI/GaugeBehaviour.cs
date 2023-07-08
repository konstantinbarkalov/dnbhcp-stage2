using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeUIBehaviour : MonoBehaviour
{
    [Range(-1,1)]
    public float bratio;
    public RectTransform positivePlate;
    public RectTransform negativePlate;
    public RectTransform neutralPlate;
    public TMPro.TMP_Text text;
    void OnValidate() {
        UpdateGauge();
    }
    void Update() {
        UpdateGauge();
    }
    void UpdateGauge() {
        float positiveRatio = Mathf.Max(0, bratio);
        float negatveRatio = Mathf.Max(0, -bratio);
        float neutralRatio = 1 - positiveRatio - negatveRatio;
        
        Vector2 positivePlateAnchorMax = positivePlate.anchorMax;
        positivePlateAnchorMax.x = positiveRatio;    
        positivePlate.anchorMax = positivePlateAnchorMax;
        
        Vector2 negativePlateAnchorMin = negativePlate.anchorMin;
        negativePlateAnchorMin.x = 1 - negatveRatio;    
        negativePlate.anchorMin = negativePlateAnchorMin;
        
        Vector2 neutralPlateAnchorMax = neutralPlate.anchorMax;
        neutralPlateAnchorMax.x = positiveRatio + neutralRatio;    
        neutralPlate.anchorMax = neutralPlateAnchorMax;
        
        Vector2 neutralPlateAnchorMin = neutralPlate.anchorMin;
        neutralPlateAnchorMin.x = positiveRatio;    
        neutralPlate.anchorMin = neutralPlateAnchorMin;
        
        text.text = Mathf.Round(bratio * 100).ToString() + "%";
        
    }
}

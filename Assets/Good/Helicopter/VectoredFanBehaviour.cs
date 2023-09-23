using Good;
using Good.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class VectoredFan2Behaviour : MonoBehaviour
{
    public VectoredFanPowerBratios powerBratios; 
    public ParticleSystem particleSystemA;
    public ParticleSystem particleSystemB;
    public SpinningFanBehavour mainSpinningFan;
    public Transform fakeInclinedFan;
    public Transform fanForceAAnchor;
    public Transform fanForceBAnchor;
    public HelicopterBehaivour helicopter;
    public AudioSource audioSource;
    public float maxFakeInclinedForewardAngle = 12;
    public float maxFakeInclinedBackwardAngle = 12;
    private float smoothInclinedAngle = 0;
    public float maxPower = 30000;
    public float maxHeight = 50;
    public Vector2 MovementAmount; // TODO: remove

    void Awake()
    {
        // DontDestroyOnLoadManager.DontDestroyOnLoad(gameObject);
        // if (navigationManager == null)
        // {
        //     Scene uiScene = SceneManager.GetSceneByName("Root");
        //     GameObject[] uiSceneGameObjects = uiScene.GetRootGameObjects();
        //     foreach (var gameObject in uiSceneGameObjects)
        //     {
        //         if (gameObject.tag == "ui_root")
        //         {
        //             gameObject.transform.SetSiblingIndex(10);
        //             navigationManager = gameObject.GetComponent<NavigationManager>();
        //         }
        //     }
        // }
        // GameObject UIRootGameObject = GameObject.FindWithTag("ui_root");
        // navigationManager = UIRootGameObject.GetComponent<NavigationManager>();
        // navigationManager.Init();
    }
    
    void Start()
    {
        // if (navigationManager == null) {
        //     Scene uiScene = SceneManager.GetSceneByName("Root");
        //     GameObject[] uiSceneGameObjects = uiScene.GetRootGameObjects();
        //     foreach (var gameObject in uiSceneGameObjects)
        //     {
        //         if (gameObject.tag == "ui_root") {
        //             gameObject.transform.SetSiblingIndex(10);
        //             navigationManager = gameObject.GetComponent<NavigationManager>();
        //         }
        //     }
        //     // GameObject UIRootGameObject = GameObject.FindWithTag("ui_root");
        //     // UIRootGameObject.transform.SetSiblingIndex(10);
        //     // navigationManager = UIRootGameObject.GetComponent<NavigationManager>();
        //     // navigationManager.Init();
        // }
            GameObject UIRootGameObject = GameObject.FindWithTag("ui_root");
            MetaManager.app.uIManager = UIRootGameObject.GetComponent<UIManager>();
            MetaManager.app.uIManager.Init();        
    }
    public float CalculateMaxPowerAtHeight() {
        float linearHeightRatio = 1 - Mathf.Clamp01(helicopter.transform.position.y / maxHeight);
        float heightRatio = Mathf.Pow(linearHeightRatio, 1/2f);
        float maxPowerAtHeight = maxPower * heightRatio;
        return maxPowerAtHeight;
    }

    public Vector3 CalculateUnitTorque() {
        Vector3 unitTorqueA = ExtendedPhysics.Utils.ForceAtPositionToTorque(helicopter.rigidBody, helicopter.rigidBody.transform.up, fanForceAAnchor.position);
        Vector3 unitTorqueB = ExtendedPhysics.Utils.ForceAtPositionToTorque(helicopter.rigidBody, -helicopter.rigidBody.transform.up, fanForceBAnchor.position);
        Vector3 unitTorque = unitTorqueA + unitTorqueB;
        return unitTorque;
    }
    

    private void UpdateFakeInclinedFan() {
        float inclinedAngleBratio = (powerBratios.a - powerBratios.b) * 10;
        float inclinedAngle;
        if (inclinedAngleBratio < 0) { 
            inclinedAngle = inclinedAngleBratio * maxFakeInclinedBackwardAngle; 
        } else { 
            inclinedAngle = inclinedAngleBratio * maxFakeInclinedForewardAngle; 
        }
        inclinedAngle = Mathf.Clamp(inclinedAngle, -maxFakeInclinedBackwardAngle, maxFakeInclinedForewardAngle);
        smoothInclinedAngle = smoothInclinedAngle * 0.95f + inclinedAngle * 0.05f;

        Quaternion inclinedRotation = Quaternion.Euler(0, 0, smoothInclinedAngle);
        fakeInclinedFan.localRotation = inclinedRotation;
    }

    private void AddForces() {
        float maxPowerAtHeight = CalculateMaxPowerAtHeight();
        float powerA = powerBratios.a * maxPowerAtHeight / 2;
        float powerB = powerBratios.b * maxPowerAtHeight / 2;
        
        Vector3 direction = helicopter.rigidBody.transform.up;
        Vector3 forceA = powerA * direction;
        Vector3 forceB = powerB * direction;

        //helicopter.rigidBody.AddForceAtPosition(forceA, fanForceAAnchor.position, ForceMode.Force);
        //helicopter.rigidBody.AddForceAtPosition(forceB, fanForceBAnchor.position, ForceMode.Force);
        ExtendedPhysics.Utils.AddForceAtPosition(helicopter.rigidBody, forceA, fanForceAAnchor.position, ForceMode.Force);
        ExtendedPhysics.Utils.AddForceAtPosition(helicopter.rigidBody, forceB, fanForceBAnchor.position, ForceMode.Force);

    }
    private void UpdateAudio() {
        float powerRatioNonlinear = (Mathf.Abs(powerBratios.a) + Mathf.Abs(powerBratios.b)) / 2 / 0.8f;
        float powerRatioNonlinear1 = Mathf.Pow(powerRatioNonlinear, 1/1f);
        float powerRatioNonlinear2 = Mathf.Pow(powerRatioNonlinear, 1/2f);
        audioSource.volume = (0.25f + powerRatioNonlinear2 * 0.75f) * 1/2f;
        float powerSign = Mathf.Sign(powerBratios.b + powerBratios.a);
        float patternFactor = powerRatioNonlinear1 < 1/4f ? 1/4f : powerRatioNonlinear1 < 1/3f ? 1/3f : powerRatioNonlinear1 < 1/2f ? 1/2f : powerRatioNonlinear1 < 2/3f ? 2/3f : powerRatioNonlinear1 < 3/4f ? 3/4f : 1;
        audioSource.pitch = patternFactor * powerSign;
        mainSpinningFan.angleVelocity = 360 * 4 * patternFactor * powerSign; 
    }
    private void UpdateParticles() {
        Color dustColor = MetaManager.level.daytimeEnvironmentManager.GetDustColor();
        var mainA = particleSystemA.main;
        mainA.startSpeed = 2000 * powerBratios.a;
        mainA.startColor = dustColor;
        var emissionA = particleSystemA.emission;
        emissionA.rateOverTime = 75 * powerBratios.a;
        var mainB = particleSystemB.main;
        mainB.startSpeed = 2000 * powerBratios.b;
        mainB.startColor = dustColor;
        var emissionB = particleSystemB.emission;
        emissionB.rateOverTime = 75 * powerBratios.b;
    }
    private void UpdateGauges() {
        float powerBratio = (Mathf.Abs(powerBratios.a) + Mathf.Abs(powerBratios.b)) / 2;
        MetaManager.app.uIManager.helicopterPowerBratio = powerBratio;
     }
    
    void FixedUpdate()
    {
        UpdateFakeInclinedFan();
        AddForces();
        UpdateAudio();
        UpdateParticles();
        UpdateGauges();
    }

}

public struct VectoredFanPowerBratios {
    public float a;
    public float b;
}

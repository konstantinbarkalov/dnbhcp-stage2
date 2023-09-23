using System;
using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(VectoredFan2Behaviour))]
public class VectoredFanControllerBehaviour : MonoBehaviour
{   
    private VectoredFan2Behaviour vectoredFan;
    public InputAction playerControls;
    private InputState inputState;
    private VectoredFanControllerMode mode;    
    private VectoredFanControllerModeRatios modeRatios;    
    void Awake()
    {
        vectoredFan = this.GetComponent<VectoredFan2Behaviour>();
    }

    
    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }
    private RawInputState GetRawInputState__Update() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 hv = new Vector2(horizontalInput, verticalInput);
        bool isPausePressed = Keyboard.current.pKey.wasPressedThisFrame;
        bool isBrakePressed = Keyboard.current.spaceKey.isPressed;
        bool isSwitchModePressed = Keyboard.current.eKey.wasPressedThisFrame;
        bool isRestatPressed = Keyboard.current.rKey.wasPressedThisFrame;
        
        return new RawInputState() {
            hv = hv,
            brake = isBrakePressed,
            pause = isPausePressed,
            mode = isSwitchModePressed,
            restart = isRestatPressed
        };
    }
    private void UpdateRawInputState__Update() {
        inputState.raw = GetRawInputState__Update();
    }
    private void UpdateInputState__FixedUpdate() {
        inputState.rawActivity = new Vector2(Mathf.Abs(inputState.raw.hv.x), Mathf.Abs(inputState.raw.hv.y));
        inputState.smoothHv.x = inputState.smoothHv.x * 0.75f + inputState.raw.hv.x * 0.25f; 
        inputState.smoothHv.y = inputState.smoothHv.y * 0.75f + inputState.raw.hv.y * 0.25f; 
        inputState.smoothActivity = new Vector2(Mathf.Abs(inputState.smoothHv.x), Mathf.Abs(inputState.smoothHv.y));
        inputState.smoothActivityMax = Mathf.Max(inputState.smoothActivity.x, inputState.smoothActivity.y);
        inputState.smootherHv.x = inputState.smootherHv.x * 0.95f + inputState.raw.hv.x * 0.05f; 
        inputState.smootherHv.y = inputState.smootherHv.y * 0.95f + inputState.raw.hv.y * 0.05f; 
        inputState.smootherActivity = new Vector2(Mathf.Abs(inputState.smootherHv.x), Mathf.Abs(inputState.smootherHv.y));
        inputState.smootherActivityMax = Mathf.Max(inputState.smootherActivity.x, inputState.smootherActivity.y);
        inputState.smoothestHv.x = inputState.smoothestHv.x * 0.995f + inputState.raw.hv.x * 0.005f; 
        inputState.smoothestHv.y = inputState.smoothestHv.y * 0.995f + inputState.raw.hv.y * 0.005f; 
        inputState.smoothestActivity = new Vector2(Mathf.Abs(inputState.smoothestHv.x), Mathf.Abs(inputState.smoothestHv.y));
        inputState.smoothestActivityMax = Mathf.Max(inputState.smoothestActivity.x, inputState.smoothestActivity.y);           
    }
    private void UpdateMode__FixedUpdate() {
        if (inputState.raw.mode) {
            SwitchMode();
        }
    }
    private void SwitchMode() {
        mode = (VectoredFanControllerMode)(((int)mode + 1) % 3);
    }
    
    
    private void UpdateModeRatios__FixedUpdate()
    {
        if (inputState.raw.brake) {
            modeRatios.stunt = 0;
            modeRatios.travel = 0;
            modeRatios.brake = 1;
        }
        else {
            if (mode == VectoredFanControllerMode.Auto) {
                float activityRatio = inputState.smootherActivity.x; // Mathf.Max(inputState.smoothActivity.x, inputState.smootherActivity.x);
                modeRatios.stunt = activityRatio;
                modeRatios.travel = 1 - activityRatio;
            } else if (mode == VectoredFanControllerMode.Stunt) {
                modeRatios.stunt = 1;
                modeRatios.travel = 0;
            } else if (mode == VectoredFanControllerMode.Travel) {
                modeRatios.stunt = 0;
                modeRatios.travel = 1;
            }
            modeRatios.brake = 0;
        }
    }
    private void ProcessControl__FixedUpdate() {
        VectoredFanPowerBratios powerBratios = Assist__FixedUpdate();
        vectoredFan.powerBratios = powerBratios;
    }
    
    private VectoredFanPowerBratios Assist__FixedUpdate() 
    {
        float distanceToFloor = vectoredFan.helicopter.transform.position.y;
        float attentionHvFactor = 4 / distanceToFloor;
        float stabilizationAngularCorrectionBudget = 0.1f * (1 - inputState.smoothActivity.x);
        float stabilizationGravityCorrectionBudget = 0.5f * (1 - inputState.smoothActivity.y) + 0.5f * inputState.smoothActivity.x;
        float horizontalInputBudget = 0.3f * inputState.smoothActivity.x;
        float verticalInputBudget = 0.5f * inputState.smoothActivity.y + 0.5f * (1 - inputState.smoothActivity.x);

        stabilizationAngularCorrectionBudget *= modeRatios.travel;
        stabilizationGravityCorrectionBudget *= modeRatios.travel;
        horizontalInputBudget *= modeRatios.stunt;
        verticalInputBudget *= Mathf.Max(modeRatios.travel, modeRatios.stunt);

        // stabilizationAngularCorrectionBudget *= attentionHvFactor;
        // stabilizationGravityCorrectionBudget /= attentionHvFactor;
        // horizontalInputBudget *= modeRatios.stunt;
        // verticalInputBudget *= Mathf.Max(modeRatios.travel, modeRatios.stunt);
        
        float budgetSum = stabilizationAngularCorrectionBudget + stabilizationGravityCorrectionBudget + horizontalInputBudget + verticalInputBudget;

        float stabilizationAngularCorrectionBudgetRatio = stabilizationAngularCorrectionBudget / budgetSum;
        float stabilizationGravityCorrectionBudgetRatio = stabilizationGravityCorrectionBudget / budgetSum;
        float horizontalInputBudgetRatio = horizontalInputBudget / budgetSum;
        float verticalInputBudgetRatio = verticalInputBudget / budgetSum;

        string debugText = "" + 
            "budgetSum: " + Mathf.Round(budgetSum * 100) + "%\r\n" +
            "stabilizationAngularCorrectionBudgetRatio: " + Mathf.Round(stabilizationAngularCorrectionBudgetRatio * 100) + "%\r\n" +
            "stabilizationGravityCorrectionBudgetRatio: " + Mathf.Round(stabilizationGravityCorrectionBudgetRatio * 100) + "%\r\n" +
            "horizontalInputBudgetRatio: " + Mathf.Round(horizontalInputBudgetRatio * 100) + "%\r\n" +
            "verticalInputBudgetRatio: " + Mathf.Round(verticalInputBudgetRatio * 100) + "%\r\n" +
            "inputState.smoothActivity.x: " + Mathf.Round(inputState.smoothActivity.x * 100) + "%\r\n" +
            "inputState.smoothActivity.y: " + Mathf.Round(inputState.smoothActivity.y * 100) + "%\r\n" +
            "inputState.smoothActivityMax: " + Mathf.Round(inputState.smoothActivityMax * 100) + "%\r\n";
        MetaManager.app.debugManager.text = debugText;

        //float stabilizationAngularCorrectionBudgetRatio = PlayerPrefs.GetFloat("stabilization-angular-correction-budget-ratio", 0.05f);
        //float stabilizationGravityCorrectionBudgetRatio = PlayerPrefs.GetFloat("stabilization-gravity-correction-budget-ratio", 0.3f);
        //float horizontalInputBudgetRatio = PlayerPrefs.GetFloat("horizontal-input-budget-ratio", 0.10f);
        //float verticalInputBudgetRatio = 1 - stabilizationAngularCorrectionBudgetRatio - stabilizationGravityCorrectionBudgetRatio - horizontalInputBudgetRatio;

        
        float targetPitchAngle = AssistCalculateTargetPitchAngle();
        Vector3 unitTorque = vectoredFan.CalculateUnitTorque();
        float maxPowerAtHeight = vectoredFan.CalculateMaxPowerAtHeight();
        float stabilizationCorrectionAngularAccelerationBudget =  maxPowerAtHeight / 2 * stabilizationAngularCorrectionBudgetRatio * unitTorque.z / vectoredFan.helicopter.rigidBody.inertiaTensor.z;
        float stabilizationAngularCorrectionBratio = AssistStabilizationAngularCorrection(targetPitchAngle, stabilizationCorrectionAngularAccelerationBudget);
        // maxPowerAtHeight divide by 2 because of unitTorque that converts both engines as 1 single input
        
        float stabilizationGravityCorrectionAccelerationBudget =  maxPowerAtHeight * stabilizationGravityCorrectionBudgetRatio / vectoredFan.helicopter.rigidBody.mass;        
        float stabilizationGravityCorrectionBratio = AssistStabilizationGravityCorrection(stabilizationGravityCorrectionAccelerationBudget);
        VectoredFanPowerBratios powerBratios = new VectoredFanPowerBratios();
        powerBratios.a = 0 + 
            + inputState.raw.hv.y * verticalInputBudgetRatio 
            + stabilizationGravityCorrectionBratio * stabilizationGravityCorrectionBudgetRatio 
            - inputState.raw.hv.x * horizontalInputBudgetRatio 
            + stabilizationAngularCorrectionBratio * stabilizationAngularCorrectionBudgetRatio;
        powerBratios.b = 0 +
            + inputState.raw.hv.y * verticalInputBudgetRatio 
            + stabilizationGravityCorrectionBratio * stabilizationGravityCorrectionBudgetRatio 
            + inputState.raw.hv.x * horizontalInputBudgetRatio 
            - stabilizationAngularCorrectionBratio * stabilizationAngularCorrectionBudgetRatio;
        return powerBratios;
    }
    private float AssistCalculateTargetPitchAngle() {
        float targetPitchAngle;
        if (inputState.raw.brake) {
            targetPitchAngle = AssistCalculateTargetPitchAngle__Brake();
        } else {
            float targetPitchAngleHorizontal = AssistCalculateTargetPitchAngle__Horizontal(inputState.raw.hv.x);
            float targetPitchAngleVertical = AssistCalculateTargetPitchAngle__Vertical(inputState.raw.hv.y);
            float hvRatioLog = Mathf.Log(Mathf.Abs(inputState.raw.hv.y) + 0.5f) - Mathf.Log(Mathf.Abs(inputState.raw.hv.x) + 0.5f);
            float hvBratio = Mathf.Clamp(hvRatioLog, -1, 1);
            float hvRatio = hvBratio / 2f +0.5f;
            //targetPitchAngle = Mathf.LerpAngle(targetPitchAngleHorizontal, targetPitchAngleVertical, hvRatio);
            targetPitchAngle = targetPitchAngleHorizontal;
        }
        return targetPitchAngle;
        
    }
    private float AssistCalculateTargetPitchAngle__Horizontal(float horizontalInputBratio) {
        float actualHorizontalVelocity = vectoredFan.helicopter.rigidBody.velocity.x;
        float targetMaxHorizontalVelocity = 50;
        float targetHorizontalVelocity = targetMaxHorizontalVelocity * horizontalInputBratio;
        float horizontalVelocityDifference = targetHorizontalVelocity - actualHorizontalVelocity;
        
        float horizontalVelocityDifferenceBratio = horizontalVelocityDifference / targetMaxHorizontalVelocity;
        horizontalVelocityDifferenceBratio = Mathf.Clamp(horizontalVelocityDifferenceBratio, -1, 1);
        
        float targetPitchAngle = (-horizontalVelocityDifferenceBratio * 90 + 360 + 180) % 360 - 180;
        return targetPitchAngle;
    }
    private float AssistCalculateTargetPitchAngle__Vertical(float verticalInputBratio) {        
        float targetPitchAngle = 0;
        return targetPitchAngle;
    }
    private float AssistCalculateTargetPitchAngle__Brake() {
        float targetPitchAngle = -Vector3.SignedAngle(vectoredFan.helicopter.rigidBody.velocity, Vector3.down, Vector3.forward);
        return targetPitchAngle;
        
    }



    private float AssistStabilizationGravityCorrection(float stabilizationGravityCorrectionAccelerationBudget) {
        float actualVelocity = vectoredFan.helicopter.rigidBody.velocity.y;
        float actualPitchAngle = vectoredFan.helicopter.rigidBody.rotation.eulerAngles.z;
        float maxAcceleration = stabilizationGravityCorrectionAccelerationBudget;
        float maxYAcceleration = maxAcceleration * Mathf.Cos(actualPitchAngle * Mathf.Deg2Rad);

        float gravityVelocityDifference = Time.fixedDeltaTime * UnityEngine.Physics.gravity.y;
        float velocityDifferenceToCompencate = actualVelocity * 0 + gravityVelocityDifference;
        float signedTimeToCompencate = velocityDifferenceToCompencate / maxYAcceleration;
        float unclampedBratio = -signedTimeToCompencate / Time.fixedDeltaTime; 
        float stabilizationCorrectionBratio =  Mathf.Clamp(unclampedBratio, -1, 1);
        
        return stabilizationCorrectionBratio;
    }
    private float AssistStabilizationAngularCorrection(float targetPitchAngle, float stabilizationCorrectionAngularAccelerationBudget) {
        float actualPitchAngle = vectoredFan.helicopter.rigidBody.rotation.eulerAngles.z;
        float actualPitchVelocity = vectoredFan.helicopter.rigidBody.angularVelocity.z;
        float vhSign = OptimalControl.CalculateSimpleAngularControl(targetPitchAngle, actualPitchAngle, actualPitchVelocity, stabilizationCorrectionAngularAccelerationBudget);
        float stabilizationCorrectionBratio = vhSign;
        return stabilizationCorrectionBratio;
    }

    void Update()
    {
        UpdateRawInputState__Update();
    }
    
    void FixedUpdate()
    {
        UpdateInputState__FixedUpdate();
        UpdateMode__FixedUpdate();
        UpdateModeRatios__FixedUpdate();
        ProcessControl__FixedUpdate();
        UpdateGauges();
    }

    private void UpdateGauges() {
        MetaManager.app.uIManager.controlModeRatios = modeRatios;
        MetaManager.app.uIManager.controlMode = mode;
    }
}

public struct RawInputState {
    public Vector2 hv;
    public bool brake;
    public bool pause;
    public bool mode;
    public bool restart;

}

public struct InputState {
    public RawInputState raw;
    public Vector2 rawActivity;
    public Vector2 smoothHv;
    public Vector2 smoothActivity;
    public float smoothActivityMax;

    public Vector2 smootherHv;
    public Vector2 smootherActivity;
    public float smootherActivityMax;
    public Vector2 smoothestHv;
    public Vector2 smoothestActivity;
    public float smoothestActivityMax;

}

public enum VectoredFanControllerMode 
{
    Auto,
    Stunt,
    Travel,
}

public struct VectoredFanControllerModeRatios {
    public float stunt;
    public float travel;
    public float brake;
}

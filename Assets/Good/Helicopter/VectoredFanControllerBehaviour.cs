using Good;
using Good.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VectoredFan2Behaviour))]
public class VectoredFanControllerBehaviour : MonoBehaviour
{   
    private VectoredFan2Behaviour vectoredFan;
    public InputAction playerControls;
    private float smoothСontrolActivityRatio = 0;

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
    
    private void ProcessControl() {

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        bool isPausePressed = Keyboard.current.spaceKey.isPressed;
        bool isBrakePressed = Keyboard.current.enterKey.isPressed;
        if (isPausePressed) {    
            //UnityEditor.EditorApplication.isPaused = true;
            // TODO: work standalone without editor
        }
        VectoredFanPowerBratios powerBratios = Assist(horizontalInput, verticalInput, isBrakePressed);
        vectoredFan.powerBratios = powerBratios;
    }
    private VectoredFanPowerBratios Assist(float horizontalInputBratio, float verticalInputBratio, bool isBrakePressed) 
    {
        //Vector2 input = new Vector2(horizontalInputBratio, verticalInputBratio);
        float controlActivityRatio = Mathf.Abs(horizontalInputBratio);
        smoothСontrolActivityRatio = smoothСontrolActivityRatio * 0.99f + controlActivityRatio * 0.01f; 
        
        float stabilizationAngularCorrectionBudgetRatio = 0.1f * (1 - smoothСontrolActivityRatio);
        float stabilizationGravityCorrectionBudgetRatio = 0.3f * (1 - smoothСontrolActivityRatio);
        float horizontalInputBudgetRatio = 0.3f * smoothСontrolActivityRatio;
        float verticalInputBudgetRatio = 1 - stabilizationAngularCorrectionBudgetRatio - stabilizationGravityCorrectionBudgetRatio - horizontalInputBudgetRatio;
        //Debug.Log(smoothСontrolActivityRatio);

        //float stabilizationAngularCorrectionBudgetRatio = PlayerPrefs.GetFloat("stabilization-angular-correction-budget-ratio", 0.05f);
        //float stabilizationGravityCorrectionBudgetRatio = PlayerPrefs.GetFloat("stabilization-gravity-correction-budget-ratio", 0.3f);
        //float horizontalInputBudgetRatio = PlayerPrefs.GetFloat("horizontal-input-budget-ratio", 0.10f);
        //float verticalInputBudgetRatio = 1 - stabilizationAngularCorrectionBudgetRatio - stabilizationGravityCorrectionBudgetRatio - horizontalInputBudgetRatio;

        
        float targetPitchAngle = AssistCalculateTargetPitchAngle(horizontalInputBratio, verticalInputBratio, isBrakePressed);
        Vector3 unitTorque = vectoredFan.CalculateUnitTorque();
        float maxPowerAtHeight = vectoredFan.CalculateMaxPowerAtHeight();
        float stabilizationCorrectionAngularAccelerationBudget =  maxPowerAtHeight / 2 * stabilizationAngularCorrectionBudgetRatio * unitTorque.z / vectoredFan.helicopter.rigidBody.inertiaTensor.z;
        float stabilizationAngularCorrectionBratio = AssistStabilizationAngularCorrection(targetPitchAngle, stabilizationCorrectionAngularAccelerationBudget);
        // maxPowerAtHeight divide by 2 because of unitTorque that converts both engines as 1 single input
        
        float stabilizationGravityCorrectionAccelerationBudget =  maxPowerAtHeight * stabilizationGravityCorrectionBudgetRatio / vectoredFan.helicopter.rigidBody.mass;        
        float stabilizationGravityCorrectionBratio = AssistStabilizationGravityCorrection(stabilizationGravityCorrectionAccelerationBudget);
        VectoredFanPowerBratios powerBratios = new VectoredFanPowerBratios();
        powerBratios.a = 0 + 
            + verticalInputBratio * verticalInputBudgetRatio 
            + stabilizationGravityCorrectionBratio * stabilizationGravityCorrectionBudgetRatio 
            - horizontalInputBratio * horizontalInputBudgetRatio 
            + stabilizationAngularCorrectionBratio * stabilizationAngularCorrectionBudgetRatio;
        powerBratios.b = 0 +
            + verticalInputBratio * verticalInputBudgetRatio 
            + stabilizationGravityCorrectionBratio * stabilizationGravityCorrectionBudgetRatio 
            + horizontalInputBratio * horizontalInputBudgetRatio 
            - stabilizationAngularCorrectionBratio * stabilizationAngularCorrectionBudgetRatio;
        return powerBratios;
    }
    private float AssistCalculateTargetPitchAngle(float horizontalInputBratio, float verticalInputBratio, bool isBrakePressed) {
        float targetPitchAngle;
        if (isBrakePressed) {
            targetPitchAngle = AssistCalculateTargetPitchAngle__Brake();
        } else {
            float targetPitchAngleHorizontal = AssistCalculateTargetPitchAngle__Horizontal(horizontalInputBratio);
            float targetPitchAngleVertical = AssistCalculateTargetPitchAngle__Vertical(verticalInputBratio);
            float hvRatioLog = Mathf.Log(Mathf.Abs(verticalInputBratio) + 0.5f) - Mathf.Log(Mathf.Abs(horizontalInputBratio) + 0.5f);
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
        float directionSign = OptimalControl.CalculateSimpleAngularControl(targetPitchAngle, actualPitchAngle, actualPitchVelocity, stabilizationCorrectionAngularAccelerationBudget);
        float stabilizationCorrectionBratio = directionSign;
        return stabilizationCorrectionBratio;
    }

    void FixedUpdate()
    {
        ProcessControl();
    }
}



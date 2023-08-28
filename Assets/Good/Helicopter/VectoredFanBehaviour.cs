using Good;
using Good.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class VectoredFan2Behaviour : MonoBehaviour
{
    public float powerABratio; 
    public float powerBBratio;
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
    public float maxPower = 3000;
    public GaugeUIBehaviour powerGauge;
    public NavigationManager navigationManager;
    public GameManager gameManager;
    public TMPro.TMP_Text debugText;
    private string debugTextKeeper;

    public MetaManagerBehaviour metaManagerBehaviour;
    public InputManagerBehaviour inputManagerBehaviour;

    public InputAction playerControls;

    public Vector2 MovementAmount;

    void Awake()
    {
        DontDestroyOnLoadManager.DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    private void GetControl() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        // float horizontalInput = MovementAmount.x;
        // float verticalInput = MovementAmount.y;
        // var moveDirection = playerControls.ReadValue<Vector2>();
        // float horizontalInput = moveDirection.x;
        // float verticalInput = moveDirection.y;
        // if (inputManagerBehaviour == null) {
        //     horizontalInput = Input.GetAxis("Horizontal");
        //     verticalInput = Input.GetAxis("Vertical");
        //     debugTextKeeper += "Keyboard input H: " + horizontalInput + " V: " + verticalInput + "\r\n";
        // } else {
        //     // var moveDirection = inputManagerBehaviour.MoveInput.normalized;
        //     // horizontalInput = moveDirection.x;
        //     // verticalInput = moveDirection.y;
            // AxisControl xAxisControl = Gamepad.current.leftStick.x;
            // AxisControl yAxisControl = Gamepad.current.leftStick.y;

            // if (xAxisControl != null) {
            //     horizontalInput = xAxisControl.value;
            // }

            // if (yAxisControl != null) {
            //     verticalInput = yAxisControl.value;
            // }

        //     // horizontalInput = Gamepad.current.leftStick.x.clamp;
        //     // verticalInput = Gamepad.current.leftStick.y.ReadValue();
        //     // horizontalInput = Gamepad.current.leftStick.x.ReadValue();
        //     // verticalInput = Gamepad.current.leftStick.y.ReadValue(); ;
        //     debugTextKeeper += "JoyStick input H: " + horizontalInput + " V: " + verticalInput + "\r\n";
        // }
        // float horizontalInput = Input.GetAxis("Horizontal");
        // float verticalInput = Input.GetAxis("Vertical");
        // var moveDirection = metaManagerBehaviour.inputManagerBehaviour.MoveInput.normalized;

        // bool isPausePressed = Input.GetKey(KeyCode.Space);
        bool isPausePressed = Keyboard.current.spaceKey.isPressed;
        // bool isBrakePressed = Input.GetKey(KeyCode.Return);
        bool isBrakePressed = Keyboard.current.enterKey.isPressed;
        if (isPausePressed) {    
            //UnityEditor.EditorApplication.isPaused = true;
        }
        debugTextKeeper += "Input H: " + horizontalInput + " V: " + verticalInput + "\r\n";
        Assist(horizontalInput, verticalInput, isBrakePressed);
    }
    private void Assist(float horizontalInputBratio, float verticalInputBratio, bool isBrakePressed) 
    {
        float stabilizationAngularCorrectionBudgetRatio = GlobalVariables.Get<float>("stabilization-angular-correction-budget-ratio", 0.05f);
        float stabilizationGravityCorrectionBudgetRatio = GlobalVariables.Get<float>("stabilization-gravity-correction-budget-ratio", 0.3f);
        float horizontalInputBudgetRatio = GlobalVariables.Get<float>("horizontal-input-budget-ratio", 0.10f);
        float verticalInputBudgetRatio = 1 - stabilizationAngularCorrectionBudgetRatio - stabilizationGravityCorrectionBudgetRatio - horizontalInputBudgetRatio;

        
        float targetPitchAngle = AssistCalculateTargetPitchAngle(horizontalInputBratio, verticalInputBratio, isBrakePressed);
        Vector3 unitTorque = CalculateUnitTorque();
        float stabilizationCorrectionAngularAccelerationBudget =  maxPower / 2 * stabilizationAngularCorrectionBudgetRatio * unitTorque.z / helicopter.rigidBody.inertiaTensor.z;
        float stabilizationAngularCorrectionBratio = AssistStabilizationAngularCorrection(targetPitchAngle, stabilizationCorrectionAngularAccelerationBudget);
        // maxPower divide by 2 because of unitTorque that converts both engines as 1 single input
        
        float stabilizationGravityCorrectionAccelerationBudget =  maxPower * stabilizationGravityCorrectionBudgetRatio / helicopter.rigidBody.mass;
        float stabilizationGravityCorrectionBratio = AssistStabilizationGravityCorrection(stabilizationGravityCorrectionAccelerationBudget);
        
        powerABratio = 0 + 
            + verticalInputBratio * verticalInputBudgetRatio 
            + stabilizationGravityCorrectionBratio * stabilizationGravityCorrectionBudgetRatio 
            - horizontalInputBratio * horizontalInputBudgetRatio 
            + stabilizationAngularCorrectionBratio * stabilizationAngularCorrectionBudgetRatio;
        powerBBratio = 0 +
            + verticalInputBratio * verticalInputBudgetRatio 
            + stabilizationGravityCorrectionBratio * stabilizationGravityCorrectionBudgetRatio 
            + horizontalInputBratio * horizontalInputBudgetRatio 
            - stabilizationAngularCorrectionBratio * stabilizationAngularCorrectionBudgetRatio;


        // powerABratio = 0 + 
        //     + verticalInputBratio * 0.8f 
        //     - horizontalInputBratio * 0.2f; 
        // powerBBratio = 0 +
        //     + verticalInputBratio * 0.8f 
        //     + horizontalInputBratio * 0.2f;


    }
    private Vector3 CalculateUnitTorque() {
        Vector3 unitTorqueA = ExtendedPhysics.Utils.ForceAtPositionToTorque(helicopter.rigidBody, helicopter.rigidBody.transform.up, fanForceAAnchor.position);
        Vector3 unitTorqueB = ExtendedPhysics.Utils.ForceAtPositionToTorque(helicopter.rigidBody, -helicopter.rigidBody.transform.up, fanForceBAnchor.position);
        Vector3 unitTorque = unitTorqueA + unitTorqueB;
        return unitTorque;
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
            debugTextKeeper += "targetPitchAngle: " + targetPitchAngle + "  hvBratio: " + hvBratio;
        }
        return targetPitchAngle;
        
    }
    private float AssistCalculateTargetPitchAngle__Horizontal(float horizontalInputBratio) {
        float actualHorizontalVelocity = helicopter.rigidBody.velocity.x;
        float targetMaxHorizontalVelocity = 50;
        float targetHorizontalVelocity = targetMaxHorizontalVelocity * horizontalInputBratio;
        float horizontalVelocityDifference = targetHorizontalVelocity - actualHorizontalVelocity;
        
        float horizontalVelocityDifferenceBratio = horizontalVelocityDifference / targetMaxHorizontalVelocity;
        horizontalVelocityDifferenceBratio = horizontalVelocityDifferenceBratio = Mathf.Clamp(horizontalVelocityDifferenceBratio, -1, 1);
        
        float targetPitchAngle = (-horizontalVelocityDifferenceBratio * 90 + 360 + 180) % 360 - 180;
        return targetPitchAngle;
    }
    private float AssistCalculateTargetPitchAngle__Vertical(float verticalInputBratio) {        
        float targetPitchAngle = 0;
        return targetPitchAngle;
    }
    private float AssistCalculateTargetPitchAngle__Brake() {
        float targetPitchAngle = -Vector3.SignedAngle(helicopter.rigidBody.velocity, Vector3.down, Vector3.forward);
        return targetPitchAngle;
        
    }



    private float AssistStabilizationGravityCorrection(float stabilizationGravityCorrectionAccelerationBudget) {
        float actualVelocity = helicopter.rigidBody.velocity.y;
        float actualPitchAngle = helicopter.rigidBody.rotation.eulerAngles.z;
        float maxAcceleration = stabilizationGravityCorrectionAccelerationBudget;
        float maxYAcceleration = maxAcceleration * Mathf.Cos(actualPitchAngle * Mathf.Deg2Rad);

        float gravityVelocityDifference = Time.fixedDeltaTime * UnityEngine.Physics.gravity.y;
        float velocityDifferenceToCompencate = actualVelocity * 0 + gravityVelocityDifference;
        float signedTimeToCompencate = velocityDifferenceToCompencate / maxYAcceleration;
        float unclampedBratio = -signedTimeToCompencate / Time.fixedDeltaTime; 
        float stabilizationCorrectionBratio =  Mathf.Clamp(unclampedBratio, -1, 1);
        debugTextKeeper += "\r\nstabilizationCorrectionBratio:" + stabilizationCorrectionBratio;
        
        return stabilizationCorrectionBratio;
    }
    private float AssistStabilizationAngularCorrection(float targetPitchAngle, float stabilizationCorrectionAngularAccelerationBudget) {
        float actualPitchAngle = helicopter.rigidBody.rotation.eulerAngles.z;
        float actualPitchVelocity = helicopter.rigidBody.angularVelocity.z;
        float directionSign = OptimalControl.CalculateSimpleAngularControl(targetPitchAngle, actualPitchAngle, actualPitchVelocity, stabilizationCorrectionAngularAccelerationBudget);
        float stabilizationCorrectionBratio = directionSign;
        return stabilizationCorrectionBratio;
    }







    private void UpdateFakeInclinedFan() {
        float inclinedAngleBratio = (powerABratio - powerBBratio) * 10;
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
        float powerA = powerABratio * maxPower / 2;
        float powerB = powerBBratio * maxPower / 2;
        
        Vector3 direction = helicopter.rigidBody.transform.up;
        Vector3 forceA = powerA * direction;
        Vector3 forceB = powerB * direction;

        //helicopter.rigidBody.AddForceAtPosition(forceA, fanForceAAnchor.position, ForceMode.Force);
        //helicopter.rigidBody.AddForceAtPosition(forceB, fanForceBAnchor.position, ForceMode.Force);
        ExtendedPhysics.Utils.AddForceAtPosition(helicopter.rigidBody, forceA, fanForceAAnchor.position, ForceMode.Force);
        ExtendedPhysics.Utils.AddForceAtPosition(helicopter.rigidBody, forceB, fanForceBAnchor.position, ForceMode.Force);

    }
    private void UpdateAudio() {
        float powerRatioNonlinear = (Mathf.Abs(powerABratio) + Mathf.Abs(powerBBratio)) / 2 / 0.8f;
        float powerRatioNonlinear1 = Mathf.Pow(powerRatioNonlinear, 1/1f);
        float powerRatioNonlinear2 = Mathf.Pow(powerRatioNonlinear, 1/2f);
        audioSource.volume = 0.25f + powerRatioNonlinear2 * 0.75f;
        float powerSign = Mathf.Sign(powerBBratio + powerABratio);
        float patternFactor = powerRatioNonlinear1 < 1/4f ? 1/4f : powerRatioNonlinear1 < 1/3f ? 1/3f : powerRatioNonlinear1 < 1/2f ? 1/2f : powerRatioNonlinear1 < 2/3f ? 2/3f : powerRatioNonlinear1 < 3/4f ? 3/4f : 1;
        audioSource.pitch = patternFactor * powerSign;
        mainSpinningFan.angleVelocity = 360 * 4 * patternFactor * powerSign; 
    }
    private void UpdateParticles() {
        Color dustColor = MetaManagerBehaviour.metaManager.daytimeEnvironmentManager.GetDustColor();
        var mainA = particleSystemA.main;
        mainA.startSpeed = 2000 * powerABratio;
        mainA.startColor = dustColor;
        var emissionA = particleSystemA.emission;
        emissionA.rateOverTime = 75 * powerABratio;
        var mainB = particleSystemB.main;
        mainB.startSpeed = 2000 * powerBBratio;
        mainB.startColor = dustColor;
        var emissionB = particleSystemB.emission;
        emissionB.rateOverTime = 75 * powerBBratio;
    }
    private void UpdateGauges() {
        powerGauge.bratio = (Mathf.Abs(powerABratio) + Mathf.Abs(powerBBratio)) / 2;
        debugText.text = "hele pitch angle: " + helicopter.rigidBody.rotation.eulerAngles.z + "\r\n" +
                         "hele vel x: " + helicopter.rigidBody.velocity.x +
                                 " y: " + helicopter.rigidBody.velocity.y + "\r\n" +
                                 "" + debugTextKeeper;

        if (navigationManager != null) {
            navigationManager.bratio = powerGauge.bratio;
            navigationManager.debugInfo = debugText.text;
        } else {
            gameManager.bratio = powerGauge.bratio;
            gameManager.debugInfo = debugText.text;
        }
    }
    
    void FixedUpdate()
    {
        debugTextKeeper = "";

        GetControl();
        UpdateFakeInclinedFan();
        AddForces();
        UpdateAudio();
        UpdateParticles();
        UpdateGauges();
    }





}



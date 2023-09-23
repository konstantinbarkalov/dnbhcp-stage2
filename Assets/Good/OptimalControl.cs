using UnityEngine;

static public class OptimalControl {
    static public float AngleMod(float angle) {
        return ((angle + 180) % 360 + 360) % 360 - 180; 
    }

    static public float CalculateSimpleLinearControl(float targetPosition, float actualPosition, float actualVelocity, float maxAcceleration) {
        float positionAtFatv = CalculateLinearFatv(actualPosition, actualVelocity, maxAcceleration);
        
        float fatvToTargetPositionDifference = targetPosition - positionAtFatv;
        float directionSign = Mathf.Sign(fatvToTargetPositionDifference);
        return directionSign;
    }
    static public float CalculateSimpleAngularControl(float targetAngle, float actualAngle, float actualAngularVelocity, float maxAngularAcceleration) {
        float angleAtFatv = CalculateAngularFatv(actualAngle, actualAngularVelocity, maxAngularAcceleration);
        
        float actualToTargetAnglularDifference = AngleMod(targetAngle - actualAngle) * Mathf.Deg2Rad;        
        float angleRatio = (maxAngularAcceleration > 0) ? 
                           Mathf.Clamp01(Mathf.Abs(actualToTargetAnglularDifference) / maxAngularAcceleration / Time.fixedDeltaTime / 10) :
                           1; 
        float angularVelocityRatio = (maxAngularAcceleration > 0) ?
                                     Mathf.Clamp01(Mathf.Abs(actualAngularVelocity) / maxAngularAcceleration / Time.fixedDeltaTime / 10) :
                                     1;
        // TODO: WTF??? / 10
        float ratio = Mathf.Clamp01(angleRatio + angularVelocityRatio);
        //Debug.Log(angleRatio +" " + angularVelocityRatio);
        float fatvToTargetAngularDifference = AngleMod(targetAngle - angleAtFatv) * Mathf.Deg2Rad;
        float directionSign = Mathf.Sign(fatvToTargetAngularDifference);
        return directionSign * ratio;
    }
    static private float CalculateLinearFatv(float actualPosition, float actualVelocity, float maxAcceleration) {
        float positionDifferenceToFatv = CalculateRawFatv(actualVelocity, maxAcceleration);
        float positionAtFatv = actualPosition + positionDifferenceToFatv;
        return positionAtFatv;
    }
    static private float CalculateAngularFatv(float actualAngle, float actualAngularVelocity, float maxAngularAcceleration) {
        float angularDifferenceToFatv = CalculateRawFatv(actualAngularVelocity, maxAngularAcceleration);
        float angleAtFatv = actualAngle + angularDifferenceToFatv * Mathf.Rad2Deg;

        return angleAtFatv;
    }
    static private float CalculateRawFatv(float actualVelocity, float maxAcceleration) {
        /// FATV = fastest arrivable target angrular velocity point
        float targetVelocity = 0;
        float velocityDifference = actualVelocity - targetVelocity;

        float signedTimeToFatv = (maxAcceleration > 0) ? 
                                 velocityDifference / maxAcceleration : 
                                 velocityDifference * float.PositiveInfinity;
        float timeToFatv = Mathf.Abs(signedTimeToFatv);
        
        float velocityIntegrationError = (maxAcceleration * Time.fixedDeltaTime * Mathf.Sign(actualVelocity) / 2);
        float positionIntegrationErrorFactor = Mathf.Pow(1 - velocityIntegrationError / velocityDifference, 2);

        float differenceToFatvAsTriangleSquare = timeToFatv * velocityDifference / 2;
        float baseToFatvAsRectangleSquare = timeToFatv * targetVelocity;
        float positionDifferenceToFatv = differenceToFatvAsTriangleSquare * positionIntegrationErrorFactor + baseToFatvAsRectangleSquare;
        return positionDifferenceToFatv;
    }
   
    
}

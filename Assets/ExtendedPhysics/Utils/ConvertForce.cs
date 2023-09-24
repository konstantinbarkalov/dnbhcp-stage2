using UnityEngine;
namespace ExtendedPhysics
{        
    static public partial class Utils
    {
        public static Vector3 ConvertForce(Vector3 value, ForceMode from, ForceMode to, float mass, float dt) 
        {
            float forceConversationFactor = CalculateForceConversationFactor(from, to, mass, dt);
            return value * forceConversationFactor;
        }

        public static float ConvertForce(float value, ForceMode from, ForceMode to, float mass, float dt) 
        {
            float forceConversationFactor = CalculateForceConversationFactor(from, to, mass, dt);
            return value * forceConversationFactor;
        }
        
        public static float CalculateForceConversationFactor(ForceMode from, ForceMode to, float mass, float dt) 
        {
            float fromFactor = 1;
            if (from == ForceMode.Impulse || from == ForceMode.VelocityChange) {
                fromFactor /= dt;
            }
            if (from == ForceMode.Acceleration || from == ForceMode.VelocityChange) {
                fromFactor *= mass;
            }
            
            // Vector3 pureForce = value * fromFactor;
            // true pure force may be calculated as a middleware, but do not need, we just use fromFactor later 
            
            float toFactor = 1;
            if (to == ForceMode.Impulse || to == ForceMode.VelocityChange) {
                toFactor *= dt;
            }
            if (to == ForceMode.Acceleration || to == ForceMode.VelocityChange) {
                toFactor /= mass;
            }
            
            return toFactor * fromFactor;
            
        }
        
  }
}
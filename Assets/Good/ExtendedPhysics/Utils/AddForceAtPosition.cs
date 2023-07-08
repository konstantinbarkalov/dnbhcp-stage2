using UnityEngine;
namespace ExtendedPhysics
{        
    static public partial class Utils
    {
        //works exactly same as an native AddForceAtPosition (used to test mostly, use ForceAtPositionToTorque, to get separated forces to process before apply same way that is in the function below) 
        public static void AddForceAtPosition(Rigidbody rigidbody, Vector3 force, Vector3 position, ForceMode forceMode = ForceMode.Force) 
        {
            Vector3 torque = ForceAtPositionToTorque(rigidbody, force, position);
            rigidbody.AddTorque(torque, forceMode);
            rigidbody.AddForce(force, forceMode);
        }
        
        // https://forum.unity.com/threads/how-to-calculate-how-much-torque-will-rigidbody-addforceatposition-add.287164/
        public static Vector3 ForceAtPositionToTorque(Rigidbody rigidbody, Vector3 force, Vector3 position) {
            Vector3 worldDifferenceVector = position - rigidbody.worldCenterOfMass;
            Vector3 worldTorque = Vector3.Cross(worldDifferenceVector, force);
            return worldTorque;
        }
        
        // not used, just for understanding
        public static Vector3 ApplyInertiaTensor(Rigidbody rigidbody, Vector3 worldTorque)
        {
            Vector3 localTorque = Quaternion.Inverse(rigidbody.rotation) * worldTorque;
            Vector3 localTorqueInertiaApplied = Div(localTorque, rigidbody.inertiaTensor);
            Vector3 worldTorqueInertiaApplied = rigidbody.rotation * localTorqueInertiaApplied; 
            return worldTorqueInertiaApplied;
        }
    
        private static Vector3 Div(Vector3 v, Vector3 v2) {
            return new Vector3(v.x / v2.x, v.y / v2.y, v.z / v2.z);
        }       
    }
}
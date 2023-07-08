using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtendedPhysics.Explosion
{
    public class ExplosionColliderBehaivour : MonoBehaviour
    {
        private float force = 500;
        void OnTriggerStay(Collider other)
        {
            if (other.attachedRigidbody != null)
            if (other.attachedRigidbody.name == "Helicopter") { Debug.Log(other.attachedRigidbody.name +" " + other.name); }
            {
                ExtendedPhysics.Damage.Receiver.ExplosionReceiverBehaviour explosionReceiver;
                if (other.attachedRigidbody.TryGetComponent<ExtendedPhysics.Damage.Receiver.ExplosionReceiverBehaviour>(out explosionReceiver))
                {
                    explosionReceiver.AddExplosionForce(force, this.transform.position, 100, 0.5f, ForceMode.Force);
                }
                else
                {
                    Rigidbody otherRigidBody = other.attachedRigidbody;
                    if (otherRigidBody != null)
                    {
                        otherRigidBody.AddExplosionForce(force, this.transform.position, 100, 0.5f, ForceMode.Force);
                    }
                }
            }
        }
    }
}
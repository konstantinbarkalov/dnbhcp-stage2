using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtendedPhysics.Explosion
{
    public class ExplosionColliderBehaivour : MonoBehaviour
    {
        public float force = 300;
        void OnTriggerStay(Collider other)
        {
            if (other.attachedRigidbody != null)
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
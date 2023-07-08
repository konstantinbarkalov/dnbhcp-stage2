using UnityEngine;
namespace ExtendedPhysics
{
    // not sure do i really need an interface since there is one shared implementayion in extendedPhysicsProperties
    public interface IExtendedPhysicsProperties
    {
        public Rigidbody Rigidbody { get; }
        public float Mass { get; }
        public float Density { get; }
        public float Volume { get; }
        public float SurfaceSquare { get; }
        public float EquatorSize { get; }
        public float AverageCrossSquare { get; }
        public float AverageCrossSize { get; }
        public Vector3 CrossSquare { get; }
        public Vector3 CrossSize { get; }
        public float GetCrossSquareTowardsDirection(Vector3 direction);
        public float GetCrossSizeTowardsDirection(Vector3 direction);
    }

    [System.Serializable]
    public struct ExtendedPhysicsProperties : ExtendedPhysics.IExtendedPhysicsProperties
    {
        [SerializeField]
        private Rigidbody rigidbody;
        [SerializeField]
        private Vector3 crossSize;
        public ExtendedPhysicsProperties(Rigidbody rigidbody, Vector3 crossSize)
        {
            this.rigidbody = rigidbody;
            this.crossSize = crossSize;
        }
        public ExtendedPhysicsProperties(Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
            Vector3 inertiaTensor = rigidbody.inertiaTensor;
            float averageInertia = inertiaTensor.magnitude;
            if (rigidbody.isKinematic || averageInertia == 0)
            {
                this.crossSize = rigidbody.transform.lossyScale;
            }
            else
            {
                Vector3 curedInertiaTensor = new Vector3 (
                    inertiaTensor.x == 0 ? averageInertia : inertiaTensor.x,
                    inertiaTensor.y == 0 ? averageInertia : inertiaTensor.y,
                    inertiaTensor.z == 0 ? averageInertia : inertiaTensor.z                    
                );
                // curing of phenomena/hack when rotation lock in rigidbody sets inertiaTensor to 0;
                // PS: why not Infinity? 
                this.crossSize = ElipsoidInertiaTensorToCrossSize(curedInertiaTensor, rigidbody.mass);
            }
        }
        public Rigidbody Rigidbody
        {
            get => rigidbody;
        }
        public float Mass
        {
            get => Rigidbody ? Rigidbody.mass : float.PositiveInfinity;
        }
        public float Density
        {
            get => Mass / Volume;
        }
        
        public float Volume
        {
            get => crossSize.x * crossSize.y * crossSize.z * 4 / 3 * Mathf.PI / 8;
        }
        public float SurfaceSquare
        {
            get
            {
                float sphereRadius = crossSize.magnitude / 2;
                float sphereSurfaceSquare = 4 * Mathf.PI * sphereRadius * sphereRadius;
                return sphereSurfaceSquare;
            }
        }
        public float EquatorSize
        {
            get
            {
                float sphereRadius = crossSize.magnitude / 2;
                float equatorSize = 2 * Mathf.PI * sphereRadius;
                return equatorSize;
            }
        }
        public Vector3 CrossSquare
        {
            get => new Vector3(crossSize.y * crossSize.z, crossSize.x * crossSize.z, crossSize.x * crossSize.y) / 4f * Mathf.PI;
        }
        public Vector3 CrossSize
        {
            get => crossSize;
        }
        public float AverageCrossSquare
        {
            get => CrossSquare.magnitude;
        }
        public float AverageCrossSize
        {
            get => crossSize.magnitude;
        }
        public float GetCrossSquareTowardsDirection(Vector3 direction)
        {
            Vector3 boundedDirection = Vector3.Scale(direction, CrossSize);
            return boundedDirection.magnitude;
        }
        public float GetCrossSizeTowardsDirection(Vector3 direction)
        {
            Vector3 boundedDirection = Vector3.Scale(direction, CrossSquare);
            return boundedDirection.magnitude;
        }

        static public ExtendedPhysicsProperties FromRigidbody(Rigidbody rigidbody)
        {
            if (rigidbody != null)
            {
                ExtendedPhysicsPropertiesBehaviour nativeBehaviour = rigidbody.GetComponent<ExtendedPhysicsPropertiesBehaviour>();
                if (nativeBehaviour != null)
                {
                    return nativeBehaviour.extendedProperties;
                }
                else
                {
                    return new ExtendedPhysicsProperties(rigidbody);
                }
            }
            else
            {
                return new ExtendedPhysicsProperties(null, Vector3.positiveInfinity);

            }

        }

        // https://en.wikipedia.org/wiki/List_of_moments_of_inertia
        static public Vector3 ElipsoidInertiaTensorToCrossSize(Vector3 inertiaTensor, float mass)
        {
            //Debug.Log("inertiaTensor");
            //Debug.Log(inertiaTensor);
            Vector3 sumOfSquaredHalfsizes = inertiaTensor / mass * 5;
            float x = Mathf.Sqrt((sumOfSquaredHalfsizes.y + sumOfSquaredHalfsizes.z - sumOfSquaredHalfsizes.x) / 2) * 2;
            float y = Mathf.Sqrt((sumOfSquaredHalfsizes.x + sumOfSquaredHalfsizes.z - sumOfSquaredHalfsizes.y) / 2) * 2;
            float z = Mathf.Sqrt((sumOfSquaredHalfsizes.x + sumOfSquaredHalfsizes.y - sumOfSquaredHalfsizes.z) / 2) * 2;
            Vector3 crossSize = new Vector3(x, y, z);
            return crossSize;
        }
        static public Vector3 CrossSizeToElipsoidInertiaTensor(Vector3 crossSize, float mass)
        {
            float x = (crossSize.y * crossSize.y + crossSize.z * crossSize.z) / 4;
            float y = (crossSize.x * crossSize.x + crossSize.z * crossSize.z) / 4;
            float z = (crossSize.x * crossSize.x + crossSize.y * crossSize.y) / 4;
            Vector3 sumOfSquaredHalfsizes = new Vector3(x, y, z);
            Vector3 inertiaTensor = sumOfSquaredHalfsizes * mass / 5;
            return inertiaTensor;
        }

    }



}
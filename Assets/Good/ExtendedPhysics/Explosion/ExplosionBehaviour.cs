using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ExtendedPhysics.Explosion
{
    public class ExplosionBehaviour : MonoBehaviour
    {
        public ExplosionColliderBehaivour explosionSphere;
        public new Light light;
        public float explodeDuration = 0.25f;
        public float particleDuration = 1f;
        public float maxSphereScale = 20;
        public float maxLightIntensity = 3000;
        public bool isExploded = false;
        private float startExplodeTime;
        void Start()
        {
            startExplodeTime = Time.fixedTime;
            light.enabled = true;
        }
        void FixedUpdate()
        {
            if (!isExploded)
            {
                if (Time.fixedTime > startExplodeTime + explodeDuration)
                {
                    isExploded = true;
                }
                else
                {
                    float explodeTime = Time.fixedTime - startExplodeTime;
                    float explodeLinearRatio = explodeTime / explodeDuration;
                    float explodeRatio = Mathf.Pow(explodeLinearRatio, 1 / 4f);
                    float explodeScale = maxSphereScale * explodeRatio;
                    float flashRatio = Mathf.Pow(1 - explodeLinearRatio, 2f);
                    explosionSphere.transform.localScale = Vector3.one * explodeScale;
                    light.intensity = flashRatio * maxLightIntensity;
                }
                if (isExploded)
                {
                    Destroy(light.gameObject);
                    Destroy(this.gameObject, particleDuration - explodeDuration);
                }
            }
        }

    }
}
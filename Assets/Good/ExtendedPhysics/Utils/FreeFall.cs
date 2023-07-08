using UnityEngine;
namespace ExtendedPhysics
{        
    static public partial class Utils
    {
        // https://www.fxyz.ru/%D1%84%D0%BE%D1%80%D0%BC%D1%83%D0%BB%D1%8B_%D0%BF%D0%BE_%D1%84%D0%B8%D0%B7%D0%B8%D0%BA%D0%B5/%D0%BC%D0%B5%D1%85%D0%B0%D0%BD%D0%B8%D0%BA%D0%B0/%D0%BA%D0%B8%D0%BD%D0%B5%D0%BC%D0%B0%D1%82%D0%B8%D0%BA%D0%B0/%D0%BF%D0%B0%D0%B4%D0%B5%D0%BD%D0%B8%D0%B5_%D1%82%D0%B5%D0%BB/%D1%81%D0%B2%D0%BE%D0%B1%D0%BE%D0%B4%D0%BD%D0%BE%D0%B5_%D0%BF%D0%B0%D0%B4%D0%B5%D0%BD%D0%B8%D0%B5/
        public static float CalculateFreeFallVelocity(float d, float g, float dt) 
        {
            float v = Mathf.Sqrt(2f * g * d); 
            float vError = g* dt / 2;
            // finite resolution integration error compensation (rough)    
            return v + vError;
        }
        public static float CalculateFreeFallDistance(float v, float g, float dt) 
        {
            
            float d = v * v / 2 / g;
            // TODO: finite resolution integration error compensation (rough)     
            return d;
        }
        
  }
}
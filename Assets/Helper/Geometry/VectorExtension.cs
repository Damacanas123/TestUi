using UnityEngine;

namespace Slime.Helper.Geometry
{
    public static class VectorExtension
    {
        private const float TwoPI = 2 * Mathf.PI;
        /// <summary>
        /// Rotates and returns a new vector. rotation is in degrees
        /// </summary>
        /// <param name="vector2"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 v, float rotation)
        {
            float radian = rotation * Mathf.PI / 180;
                return new Vector2(
                    v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian),
                    v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian)
                );
        }

        public static float FindAngleBetweenDegree(this Vector2 vector2, Vector2 other)
        {
            return 360 * FindAngleBetweenRadian(vector2, other) / TwoPI;
        }

        public static float FindAngleBetweenRadian(this Vector2 vector2, Vector2 other)
        {
            float angle = Mathf.Atan2(other.y, other.x) - Mathf.Atan2(vector2.y, vector2.x);

            if(angle < 0) {
                angle += TwoPI;
            }

            return angle;
        }

        public static bool DoesItMakeRightTurn(this Vector2 vector2, Vector2 other)
        {
            return CrossProduct(vector2,other) < 0;
        }

        public static float CrossProduct(this Vector2 vector2, Vector2 other)
        {
            return vector2.x * other.y - vector2.y * other.x;
        }
        
        public static float CrossProduct(this Vector3 vector2, Vector3 other)
        {
            return vector2.x * other.y - vector2.y * other.x;
        }

        public static Vector2 GetReflection(this Vector2 vector2, Vector2 other)
        {
            var normalized = vector2 / vector2.magnitude;
            return other - 2 * Vector2.Dot(normalized, other) * normalized;
        }
    }
}
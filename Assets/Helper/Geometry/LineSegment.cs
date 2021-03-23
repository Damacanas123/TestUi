using UnityEngine;

namespace Slime.Helper.Geometry
{
    public class LineSegment
    {
        public Vector2 A;
        public Vector2 B;
        public Line Line;

        /// <summary>
        /// An arbitrary normal vector that is perpendicular to the line which is implied by this line segment
        /// </summary>
        public Vector2 normal => Line.normal;

        public LineSegment(Vector2 A, Vector2 B)
        {
            this.A = A;
            this.B = B;
            Line = new Line(A, B - A);
        }

        public Vector2 GetIntersection(LineSegment other,out bool doesIntersect)
        {
            var intersectionPoint = Line.GetIntersection(other.Line, out doesIntersect);
            if (!doesIntersect)
            {
                return Vector2.positiveInfinity;
            }

            if (IsInside(intersectionPoint) && other.IsInside(intersectionPoint))
            {
                return intersectionPoint;
            }

            doesIntersect = false;
            return Vector2.positiveInfinity;
        }

        public Vector2 GetIntersection(Line line,out bool doesIntersect)
        {
            var intersectionPoint = Line.GetIntersection(line, out doesIntersect);
            if (!doesIntersect)
            {
                return Vector2.positiveInfinity;
            }

            if (IsInside(intersectionPoint))
            {
                return intersectionPoint;
            }

            doesIntersect = false;
            return Vector2.positiveInfinity;
        }

        /// <summary>
        /// Checks if a given point is on line segment given that the point is on the line implied by this line segment
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsInside(Vector2 point)
        {
            var vec1 = A - point;
            var vec2 = B - point;
            var dotProduct = Vector2.Dot(vec1 / vec1.magnitude, vec2 / vec2.magnitude);
            if (Mathf.Approximately(dotProduct, -1))
            {
                return true;
            }

            return false;
        }
        
        
    }
}
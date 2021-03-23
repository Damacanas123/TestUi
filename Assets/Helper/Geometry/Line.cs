using UnityEngine;

namespace Slime.Helper.Geometry
{
    public class Line
    {
        public Vector2 Point { get; private set; }
        public Vector2 Direction { get; private set; }
        public float slope { get; private set; }
        public float offset { get; private set; }
        public Vector2 normal { get; private set; }

        public Line(Vector2 point, Vector2 direction)
        {
            Point = point;
            Direction = direction;
            CalculateOpenFormula();
            CalculateNormal();
        }

        private void CalculateOpenFormula()
        {
            slope = Direction.y / Direction.x;
            offset = Point.y - slope * Point.x;
        }

        private void CalculateNormal()
        {
            if (Mathf.Approximately(Direction.y, 0f))
            {
                normal = Vector2.up;
                return;
            }
            float x = 1;
            float y = -Direction.x * x / Direction.y;
            normal = new Vector2(x,y);
            if (!Direction.DoesItMakeRightTurn(normal))
            {
                normal *= -1;
            }
        }

        public float GetY(float x)
        {
            return slope * x + offset;
        }

        public Vector2 GetIntersection(Line other, out bool doesIntersect)
        {
            doesIntersect = false;
            //check perpendicular line conditions
            //are both lines perpendicular
            if (float.IsInfinity(slope) && float.IsInfinity(other.slope))
            {
                return Vector2.positiveInfinity;
            }
            //first line perpendicular?
            if (float.IsInfinity(slope))
            {
                var x = Point.x;
                var y = other.GetY(x);
                doesIntersect = true;
                return new Vector2(x, y);
            }
            //second line perpendicular
            if (float.IsInfinity(other.slope))
            {
                var x = other.Point.x;
                var y = GetY(x);
                doesIntersect = true;
                return new Vector2(x, y);
            }
            var dotProduct = Vector2.Dot(Direction / Direction.magnitude, other.Direction/ other.Direction.magnitude);

            if (Mathf.Approximately(Mathf.Abs(dotProduct), 1))
            {
                //they are parallel return a sentinel value
                return Vector2.positiveInfinity;
            }

            var intersectionX = (offset - other.offset) / (other.slope - slope);
            var intersectionY = GetY(intersectionX);
            doesIntersect = true;
            return new Vector2(intersectionX, intersectionY);
        }
    }
}
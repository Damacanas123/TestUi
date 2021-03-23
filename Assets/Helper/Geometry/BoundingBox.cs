using System;
using System.Collections.Generic;
using UnityEngine;

namespace Slime.Helper.Geometry
{
    public class BoundingBox
    {
        public Vector2 LeftBottom;
        public Vector2 LeftTop;
        public Vector2 RightTop;
        public Vector2 RightBottom;
        public List<LineSegment> Edges { get; private set; } = new List<LineSegment>();

        public BoundingBox(Vector2 leftBottom,Vector2 leftTop, Vector2 rightTop,Vector2 rightBottom)
        {
            LeftBottom = leftBottom;
            LeftTop = leftTop;
            RightTop = rightTop;
            RightBottom = rightBottom;
            AddEdges();
        }

        private void AddEdges()
        {
            Edges.Add(new LineSegment(LeftBottom,RightBottom));
            Edges.Add(new LineSegment(RightBottom,RightTop));
            Edges.Add(new LineSegment(RightTop,LeftTop));
            Edges.Add(new LineSegment(LeftTop,LeftBottom));
        }

        public bool CheckInside(Vector2 point)
        {
            return point.x >= LeftBottom.x &&
               point.y >= LeftBottom.y &&
               point.x <= RightTop.x &&
               point.y <= RightTop.y;
        }

        public Line FindClosestIntersection(Line line)
        {
            var intersections = FindIntersections(line);
            if (intersections.Count == 0)
            {
                return null;
            }
            intersections.Sort(new Comparison<Line>((o1, o2) =>
            {
                var o1Diff = (line.Point - o1.Point).magnitude;
                var o2Diff = (line.Point - o2.Point).magnitude;
                if (o1Diff < o2Diff)
                {
                    return -1;
                }
                if (o1Diff > o2Diff)
                {
                    return 1;
                }

                return 0;
            }));
            return intersections[0];
        }

        public List<Line> FindIntersections(Line line)
        {
            List<Line> intersections = new List<Line>();
            bool doesIntersect;
            Vector2 intersection;
            foreach (var edge in Edges)
            {
                intersection = edge.GetIntersection(line, out doesIntersect);
                if (doesIntersect)
                {
                    intersections.Add(new Line(intersection,edge.normal));
                }    
            }

            return intersections;

        }

        public void Scale(float ratio)
        {
            Edges.Clear();
            LeftBottom *= ratio;
            LeftTop *= ratio;
            RightTop *= ratio;
            RightBottom *= ratio;
            AddEdges();
        }
    }
}
using UnityEngine;

namespace Slime.Helper.GameHelper
{
    public class CoordinateHelper
    {
        private Camera _camera;

        public CoordinateHelper(Camera camera)
        {
            _camera = camera;
        }
        public Vector3 ScreenToWorldPointNormalizedToCamera(Vector3 screenPos)
        {
            return _camera.ScreenToWorldPoint(screenPos) - _camera.transform.position;
        }

        public Vector3 ScreenToWorldPoint(Vector3 screenPos)
        {
            return _camera.ScreenToWorldPoint(screenPos);
        }
    }
}
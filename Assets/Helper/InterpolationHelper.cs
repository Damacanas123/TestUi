namespace Slime.Helper
{
    public static class InterpolationHelper
    {
        public static float Square(float min, float max, float ratio)
        {
            var interpolation = - ratio * ratio + 2 * ratio;
            return min + (max - min) * interpolation;
        }
        
        public static float Square(float ratio)
        {
            return - ratio * ratio + 2 * ratio;
        }

        public static float Linear(float min, float max, float ratio)
        {
            return (max - min) * ratio + min;
        }
    }
}
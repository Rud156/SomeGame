using Godot;

namespace SomeGame.Helpers
{
    public static class MathUtils
    {
        private const float FloatTolerance = 0.1f;

        public static bool IsNearlyEqual(float a, float b)
        {
            return Mathf.Abs(a - b) <= FloatTolerance;
        }

        public static bool IsNearlyZero(float a)
        {
            return Mathf.Abs(a) <= FloatTolerance;
        }

        public static bool IsNearlyEqualTol(float a, float b, float tolerance)
        {
            return Mathf.Abs(a - b) <= tolerance;
        }

        public static float To360Angle(float angle)
        {
            while (angle < 0)
                angle += 360.0f;

            while (angle >= 360.0f)
                angle -= 360.0f;

            return angle;
        }
    }
}
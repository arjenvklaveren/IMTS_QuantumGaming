using UnityEngine;

namespace Game.Data
{
    public static class OrientationUtils
    {
        #region Rotation
        public static Orientation RotateClockwise(this Orientation orientation, int increments = 1)
        {
            int dir = (int)orientation;

            // Rotate in increments of 90 degrees.
            // Keep result in 0 - 3 range and handle negative values.
            dir = (dir + ((increments % 4) + 4)) % 4;

            return (Orientation)dir;
        }

        public static Orientation RotateCounterClockwise(this Orientation orientation, int increments = 1)
        {
            return orientation.RotateClockwise(-increments);
        }
        #endregion

        #region Cast
        public static Vector2 ToVector2(this Orientation orientation)
        {
            return orientation switch
            {
                Orientation.Up => Vector2.up,
                Orientation.Right => Vector2.right,
                Orientation.Down => Vector2.down,
                Orientation.Left => Vector2.left,
                _ => Vector2.zero
            };
        }

        public static Vector2Int ToVector2Int(this Orientation orientation)
        {
            return orientation switch
            {
                Orientation.Up => Vector2Int.up,
                Orientation.Right => Vector2Int.right,
                Orientation.Down => Vector2Int.down,
                Orientation.Left => Vector2Int.left,
                _ => Vector2Int.zero
            };
        }
        #endregion
    }
}

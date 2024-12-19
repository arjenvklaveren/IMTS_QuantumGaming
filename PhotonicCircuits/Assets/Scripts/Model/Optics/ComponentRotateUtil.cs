using Game.Data;

namespace Game
{
    public static class ComponentRotateUtil
    {
        public static void SetOrientation(OpticComponent component, Orientation targetOrientation)
        {
            int incrementsToRotate = component.orientation.GetClockwiseIncrementsDiff(targetOrientation);

            if (incrementsToRotate == 0)
                return;

            GridController.TryRotateComponentClockwise(component, incrementsToRotate);
        }

        private static GridController GridController => GridManager.Instance.GridController;
    }
}

using Game.Data;
using UnityEngine;

namespace Game.Model
{
    public static class SelectedComponentTester
    {
        private static GridData data = new GridData("Test", new Vector2(0,0), new Vector2Int(0,0));
        private static Vector2Int[] tiles = new Vector2Int[] { new Vector2Int(6,9) };
        private static ComponentPort[] ports = new ComponentPort[] { };
        public static OpticComponent selectedComponent = new PhaseShifterComponent(data, tiles, Orientation.Right, ports, ports, 0);
    }
}

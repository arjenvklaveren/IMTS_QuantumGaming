using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "mirrorPlaceData", menuName = "ScriptableObjects/Components/Place Data/mirror")]
    public class MirrorComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new MirrorComponent(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts);
        }
    }
}
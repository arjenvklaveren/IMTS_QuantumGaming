using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "mirrorPlaceData", menuName = "ScriptableObjects/Components/Place Data/mirror")]
    public class MirrorComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(Vector2Int[] tilesToOccupy)
        {
            return new MirrorComponent(
                tilesToOccupy,
                inPorts,
                outPorts);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Data;

namespace Game
{
    public class ICPhaseShifterComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.ICPhaseShifter;

        public ICPhaseShifterComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ComponentPort[] inPorts, 
            ComponentPort[] outPorts
            ) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts)
        {

        }
    }
}

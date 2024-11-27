using Game.Data;
using System;
using UnityEngine;

namespace Game
{
    public class ICInComponent : OpticComponent
    {
        public event Action<Photon> OnHandlePhoton;

        public override OpticComponentType Type => OpticComponentType.ICIn;

        public ICInComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts)
        {
        }

        public void HandlePhoton(Photon photon)
        {
            photon.SetPosition(occupiedRootTile);
            photon.SetPropagation(orientation);

            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);

            OnHandlePhoton?.Invoke(photon);
        }
    }
}

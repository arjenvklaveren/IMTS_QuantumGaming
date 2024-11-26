using Game.Data;
using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public abstract class ICComponentBase : OpticComponent
    {
        public event Action<Photon> OnPhotonExitIC;

        public readonly GridData internalGrid;

        protected ICComponentBase(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts,
            Vector2Int internalGridSize,
            Vector2 internalGridSpacing,
            string title) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts)
        {
            internalGrid = new(title, internalGridSpacing, internalGridSize);
        }

        #region Handle Photon
        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            yield break;
        }
        #endregion

        #region Serialization
        public override string SerializeArgs()
        {
            return "";
        }
        #endregion
    }
}

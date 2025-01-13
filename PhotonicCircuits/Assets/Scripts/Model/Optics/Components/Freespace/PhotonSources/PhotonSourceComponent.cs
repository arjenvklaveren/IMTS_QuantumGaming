using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class PhotonSourceComponent : OpticComponent
    {
        public Action<Photon> OnCreatePhoton;

        public override OpticComponentType Type => throw new System.NotImplementedException();

        public PhotonSourceComponent(
           GridData hostGrid,
           Vector2Int[] tilesToOccupy,
           Orientation defaultOrientation,
           Orientation placeOrientation,
           ComponentPort[] inPorts,
           ComponentPort[] outPorts
           ) : base(
               hostGrid,
               tilesToOccupy,
               defaultOrientation,
               placeOrientation,
               inPorts,
               outPorts)
        {
            SetupListeners();
        }

        #region Handle Events
        protected void SetupListeners()
        {
            SimulationManager.OnSimulationStart += SimulationManager_OnSimulationStart;
        }

        public override void Destroy()
        {
            SimulationManager.OnSimulationStart -= SimulationManager_OnSimulationStart;
        }

        protected void SimulationManager_OnSimulationStart()
        {
            CreatePhoton();
        }
        #endregion

        protected virtual void CreatePhoton() { }

        public string GetUniqueSourceKey()
        {
            return HostGrid.gridName + "_" + occupiedRootTile.x + "_" + occupiedRootTile.y + "_" + Type;
        }

        public override void SetOrientation(Orientation orientation) => ComponentRotateUtil.SetOrientation(this, orientation);
    }
}

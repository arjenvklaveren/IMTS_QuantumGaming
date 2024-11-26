using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class ICComponentBase : OpticComponent
    {
        public event Action<Photon> OnPhotonExitIC;

        public readonly GridData internalGrid;

        protected List<ICInComponent> inComponents;
        protected List<ICOutComponent> outComponents;

        #region Constructor
        protected ICComponentBase(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            Vector2Int internalGridSize,
            Vector2 internalGridSpacing,
            string title) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                new ComponentPort[0],
                new ComponentPort[0])
        {
            internalGrid = new(title, internalGridSpacing, internalGridSize, true);
            SetDefaultValues();
            SetupListeners();
        }

        private void SetDefaultValues()
        {
            inComponents = new();
            outComponents = new();
        }

        private void SetupListeners()
        {
            internalGrid.OnComponentAdded += GridData_OnComponentAdded;
            internalGrid.OnComponentRemoved += GridData_OnComponentRemoved;
        }
        #endregion

        #region Handle Events
        private void GridData_OnComponentAdded(OpticComponent component) => TryAddPortHandlerComponent(component);

        private void GridData_OnComponentRemoved(OpticComponent component) => TryRemovePortHandlerComponent(component);

        private void TryAddPortHandlerComponent(OpticComponent component)
        {
            if (component.Type == OpticComponentType.ICIn)
                AddInComponent(component as ICInComponent);
            else if (component.Type == OpticComponentType.ICOut)
                AddOutComponent(component as ICOutComponent);
        }

        private void AddInComponent(ICInComponent component)
        {
            inComponents.Add(component);
        }

        private void AddOutComponent(ICOutComponent component)
        {
            component.portId = outComponents.Count;
            outComponents.Add(component);

            // add listener to out component event
        }

        private void TryRemovePortHandlerComponent(OpticComponent component)
        {
            // try remove

            // if out component, don't forget to remove event listeners
        }
        //private void TryRemovePortHandlerComponent(OpticComponent component)
        //{
        //    if (component.Type == OpticComponentType.ICIn)
        //        TryRemoveIOComponent(inComponents, component as ICInComponent);

        //    else if (component.Type == OpticComponentType.ICOut)
        //        TryRemoveIOComponent(outComponents, component as ICOutComponent);
        //}

        //private void TryRemoveIOComponent<T>(List<T> components, T componentToRemove) where T : ICIOComponentBase
        //{
        //    if (!components.Contains(componentToRemove))
        //        return;

        //    components.Remove(componentToRemove);

        //    // Recalculate port Ids
        //    for (int i = 0; i < components.Count; i++)
        //        components[i].portId = i;
        //}
        #endregion

        #region Handle Photon
        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            photon.currentGrid = internalGrid;
            inComponents[port.portId].HandlePhoton(photon);

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

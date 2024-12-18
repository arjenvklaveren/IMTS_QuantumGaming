using DG.Tweening;
using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WaveGuideCornerComponent : WaveGuideComponent
    {
        public override OpticComponentType Type => OpticComponentType.WaveGuideCorner;

        public event Action<bool> OnChangeAltType;

        [ComponentContext("Alt corner", "OnChangeCornerType")] public bool isAltCorner = false;

        public WaveGuideCornerComponent(
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
            if(isAltCorner) OnChangeCornerType(isAltCorner);
        }

        public override void SetOrientation(Orientation orientation)
        {
            GridManager.Instance.GridController.TryRotateComponentClockwise(this, this.orientation.GetIncrementsDiff(orientation));
        }

        public void OnChangeCornerType(bool isAlt)
        {
            if (isAltCorner == isAlt) return;

            isAltCorner = isAlt;
            OnChangeAltType?.Invoke(isAltCorner);

            if (isAlt)
            {
                OutPorts[1].orientation = OutPorts[1].orientation.Subtract(1);
                InPorts[1].orientation = OutPorts[1].orientation.Subtract(1);
                Debug.Log(OutPorts[1].orientation);
            }
            else
            {
                OutPorts[1].orientation = OutPorts[1].orientation.Add(1);
                InPorts[1].orientation = OutPorts[1].orientation.Add(1);
                Debug.Log(OutPorts[1].orientation);
            }
        }

        public override ComponentPort GetOutPort(int inPortIndex)
        {
            return inPortIndex switch
            {
                0 => OutPorts[1],
                1 => OutPorts[0],
                _ => throw new ArgumentException("Invalid inPort")
            };
        }

        public override Vector2[] GetNodesByInPortIndex(int inPortIndex)
        {
            return inPortIndex switch
            {
                0 => new Vector2[] { pathNodes[0].position, pathNodes[1].position, },
                1 => new Vector2[] { pathNodes[1].position, pathNodes[0].position, },
                _ => throw new ArgumentException("Invalid inPort")
            };
        }
    }
}

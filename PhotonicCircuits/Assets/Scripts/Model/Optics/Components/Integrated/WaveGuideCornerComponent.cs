using Game.Data;
using System;
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
            if (isAltCorner) OnChangeCornerType(isAltCorner);
        }

        public override void SetOrientation(Orientation orientation) => ComponentRotateUtil.SetOrientation(this, orientation);

        public void OnChangeCornerType(bool isAlt)
        {
            if (isAltCorner == isAlt) return;

            isAltCorner = isAlt;
            OnChangeAltType?.Invoke(isAltCorner);

            if (isAlt)
            {
                OutPorts[1].orientation = OutPorts[1].orientation.Subtract(1);
                InPorts[1].orientation = InPorts[1].orientation.Subtract(1);
                Debug.Log(OutPorts[1].orientation);
            }
            else
            {
                OutPorts[1].orientation = OutPorts[1].orientation.Add(1);
                InPorts[1].orientation = InPorts[1].orientation.Add(1);
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

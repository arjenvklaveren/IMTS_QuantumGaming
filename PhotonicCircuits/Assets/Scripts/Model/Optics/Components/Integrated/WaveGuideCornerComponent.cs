using Game.Data;
using System;
using UnityEngine;

namespace Game
{
    public class WaveGuideCornerComponent : WaveGuideComponent
    {
        public override OpticComponentType Type => OpticComponentType.WaveGuideCorner;
        public enum CornerType { Default, DefaultFlipped, Alternative  }

        [ComponentContext("Corner Type", nameof(SetCornerType))]
        public CornerType cornerType = CornerType.Default;

        public event Action<CornerType> OnChangeCornerType;

        public WaveGuideCornerComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation defaultOrientation,
            Orientation placeOrientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts,
            CornerType cornerType
            ) : base(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts)
        {
            this.cornerType = cornerType;
            if(cornerType != CornerType.Default) SetCornerType(cornerType);
        }

        public override void SetOrientation(Orientation orientation) => ComponentRotateUtil.SetOrientation(this, orientation);

        public void SetCornerType(CornerType cornerType)
        {
            OnChangeCornerType.Invoke(cornerType);
            this.cornerType = cornerType;
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

        public override string SerializeArgs()
        {
            return cornerType.ToString();
        }
    }
}

using Game.Data;
using System;
using UnityEngine;
using Newtonsoft.Json;

namespace Game
{
    public class WaveGuideCornerComponent : WaveGuideComponent
    {
        public override OpticComponentType Type => OpticComponentType.WaveGuideCorner;

        public enum CornerType { Default, Flipped, Alternative  }
        public CornerType cornerType = CornerType.Default;

        public WaveGuideCornerComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation defaultOrientation,
            Orientation placeOrientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts,
            CornerType cornerType,
            float[] nodePathLengths
            ) : base(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts, 
                nodePathLengths)
        {
            this.cornerType = cornerType;
        }

        public override void SetOrientation(Orientation orientation) => ComponentRotateUtil.SetOrientation(this, orientation);

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
            var args = new
            {
                NodePathLengths = nodePathLengths,
                CornerType = cornerType
            };

            return JsonConvert.SerializeObject(args);
        }
    }
}

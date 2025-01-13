using Game.Data;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "waveGuideStraightPlaceData", menuName = "ScriptableObjects/Components/Place Data/waveguide straight")]
    public class WaveGuideComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public float[] nodePathLengths = Array.Empty<float>();

        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new WaveGuideComponent(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts,
                nodePathLengths);
        }

        public void SetNodePathLengths(float[] nodePathLengths)
        {
            if (AreIdenticalNodeLengths(this.nodePathLengths, nodePathLengths)) return;
            else
            {
                this.nodePathLengths = nodePathLengths;
                #if UNITY_EDITOR
                    EditorUtility.SetDirty(this); 
                    AssetDatabase.SaveAssets();
                #endif
            }
        }

        private bool AreIdenticalNodeLengths(float[] ar1, float[] ar2)
        {
            if(ar1.Length != ar2.Length) return false;
            for(int i = 0; i < ar1.Length; i++) 
            {
                if (ar1[i] != ar2[i]) return false;
            }
            return true;
        }
    }
}

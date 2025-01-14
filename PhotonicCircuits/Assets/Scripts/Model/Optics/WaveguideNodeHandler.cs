using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Data
{
    public class NodeAction
    {
        public Vector2 node;
        public Photon photon;
        public UnityAction action;

        public NodeAction(Vector2 node, Photon photon, UnityAction action)
        {
            this.node = node;
            this.photon = photon;
            this.action = action;
        }
    }

    public class WaveguideNodeHandler
    {
        private Dictionary<Vector2, List<NodeAction>> nodes = new Dictionary<Vector2, List<NodeAction>>();
        private List<List<int>> nodePathsIndexes = new List<List<int>>();
        bool hasSetup;

        WaveGuideComponent sourceComponent;

        public WaveguideNodeHandler(WaveGuideComponent source)
        {
            sourceComponent = source;
            GridController.OnGridChanged += GridController_OnGridChanged;
        }
        void GridController_OnGridChanged(GridData data)
        {
            //TEMP FIX, WORKS ONLY IF ALL GRID NAMES, INCLUDING BLUEPRINTS ARE UNIQUE, FOR WHATEVER REASON COMPARING CLASS DOES NOT WORK?!?!
            if (sourceComponent.HostGrid.gridName != data.gridName) Reset();
        }

        public void SetupNodes(List<Vector2> nodes, List<List<int>> nodePaths, bool mirrorPaths = true)
        {
            this.nodes.Clear();
            foreach (Vector2 node in nodes)
            {
                Vector2 roundedNode = new Vector2(MathF.Round(node.x, 3), MathF.Round(node.y, 3));
                this.nodes.Add(roundedNode, new List<NodeAction>());
            }

            nodePathsIndexes = nodePaths;
            if (!mirrorPaths) return;
            foreach (List<int> nodePath in nodePaths.ToList())
            {
                nodePathsIndexes.Add(nodePath.AsEnumerable().Reverse().ToList());
            }

            hasSetup = true;
        }

        public bool HasSetup() { return hasSetup; }
        public List<Vector2> GetAllNodes() { return nodes.Keys.ToList(); }

        public void AddNodeAction(NodeAction action, bool instantIfNotSetup = true)
        {
            if (!hasSetup && instantIfNotSetup) { action.action.Invoke(); return; }
            Vector2 roundedNode = new Vector2(MathF.Round(action.node.x, 3), MathF.Round(action.node.y, 3));
            if (!nodes.ContainsKey(roundedNode))
            {
                Debug.LogError("Node does not exist!"); return; 
            }
            nodes[action.node].Add(action);
        }

        public void ExecuteNodeActions(Photon photon, Vector2 node) 
        { 
            foreach(NodeAction action in nodes[node].ToList())
            {
                if (action.photon == photon)
                {
                    action.action.Invoke();
                    nodes[node].Remove(action);
                }
            }
        }

        public List<Vector2> GetNodePath(Vector2 startNode, Vector2 endNode, Vector2? middleNode = null)
        {
            if(!nodes.ContainsKey(startNode) || !nodes.ContainsKey(endNode)) return null;
            if (middleNode.HasValue) if (!nodes.ContainsKey(middleNode.Value)) return null;

            Dictionary<int, Vector2Int> algDict = new Dictionary<int, Vector2Int>();

            for(int i = 0; i < nodePathsIndexes.Count; i++)
            {
                Vector2? sNode = null;
                Vector2? mNode = null;
                Vector2Int nodesIndexes = new Vector2Int(0, 0);

                for (int j = 0;  j < nodePathsIndexes[i].Count; j++)
                {
                    Vector2 cNode = nodes.ElementAt(nodePathsIndexes[i][j]).Key;

                    if (cNode == startNode) { sNode = cNode; nodesIndexes.x = j; }
                    if (middleNode.HasValue) if (cNode == middleNode.Value) mNode = middleNode.Value;
                    if (cNode == endNode && sNode.HasValue) 
                    {
                        if (middleNode.HasValue) if (!mNode.HasValue) break;
                        nodesIndexes.y = j;
                        algDict.Add(i, nodesIndexes);
                        break;
                    }
                }
            }
            if (algDict.Count == 0) return null;

            algDict.OrderByDescending(x => (x.Value.y - x.Value.x));
            KeyValuePair<int, Vector2Int> algPair = algDict.ElementAt(0);
            int nodePathIndex = algPair.Key;

            List<Vector2> returnList = GetPartOfNodePath(nodePathIndex, algPair.Value);
            return returnList;
        }

        private List<Vector2> GetPartOfNodePath(int nodePathIndex, Vector2Int range)
        {
            List<Vector2> returnList = new List<Vector2>();
            for(int i = range.x; i <= range.y; i++)
            {
                returnList.Add(nodes.ElementAt(nodePathsIndexes[nodePathIndex][i]).Key);
            }
            return returnList;
        }

        public List<float> GetAllNodePathLengths()
        {
            List<float> pathLengths = new List<float> ();
            for(int i = 0; i < nodePathsIndexes.Count / 2; i++)
            {
                pathLengths.Add(GetPathLength(i));
            }
            return pathLengths;
        }

        public float GetPathLength(int index)
        {
            float length = 0;
            for(int i = 0; i < nodePathsIndexes[index].Count - 1; i++)
            {
                Vector2 leftNode = nodes.ElementAt(nodePathsIndexes[index][i]).Key;
                Vector2 rightNode = nodes.ElementAt(nodePathsIndexes[index][i + 1]).Key;
                float dist = Vector2.Distance(leftNode, rightNode);
                length += dist;
            }
            return length;
        }

        public WaitForSeconds GetExitWaitTime(Photon photon, float[] nodePathLengths)
        {
            //ALWAYS TAKES FIRST LENGTH INDEX, SO CURRENTLY, ONLY SYMETRICAL PATH DESIGNS WORK, WHERE EACH PATH IS OF SAME LENGTH
            int pathIndex = 0;
            float pathLength = nodePathLengths.Length == 0 ? 0 : nodePathLengths[pathIndex];
            return PhotonMovementManager.Instance.GetWaitMoveTime(photon.GetPhotonType(), false, pathLength);
        }

        public void Reset()
        {
            nodes.Clear();
            nodePathsIndexes.Clear();
            hasSetup = false;
        }
    }
}

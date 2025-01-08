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
        private List<List<Vector2>> nodePaths = new List<List<Vector2>>();
        bool hasSetup;

        public void SetupNodes(List<Vector2> nodes, List<List<Vector2>> nodePaths)
        {
            this.nodes.Clear();
            foreach (Vector2 node in nodes) this.nodes.Add(node, new List<NodeAction>());
            this.nodePaths = nodePaths;
            hasSetup = true;
        }

        public bool HasSetup() { return hasSetup; }
        public List<Vector2> GetAllNodes() { return nodes.Keys.ToList(); }

        public void AddNodeAction(NodeAction action)
        {
            if (!nodes.ContainsKey(action.node)) { Debug.LogError("Node does not exist!"); return; }
            nodes[action.node].Add(action);
        }

        public void ExecuteNodeActions(Photon photon, Vector2 node)
        {
            foreach (NodeAction action in nodes[node].ToList())
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
            if (!nodes.ContainsKey(startNode) || !nodes.ContainsKey(endNode)) return null;
            if (middleNode.HasValue) if (!nodes.ContainsKey(middleNode.Value)) return null;

            Dictionary<int, Vector2Int> algDict = new Dictionary<int, Vector2Int>();

            for (int i = 0; i < nodePaths.Count; i++)
            {
                Vector2? sNode = null;
                Vector2? mNode = null;
                Vector2Int nodesIndexes = new Vector2Int(0, 0);

                for (int j = 0; j < nodePaths[i].Count; j++)
                {
                    Vector2 cNode = nodePaths[i][j];

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
            List<Vector2> returnList = GetPartOfNodePath(algPair.Key, algPair.Value);
            return returnList;
        }

        private List<Vector2> GetPartOfNodePath(int nodePathIndex, Vector2Int range)
        {
            List<Vector2> returnList = new List<Vector2>();
            for (int i = range.x; i <= range.y; i++)
            {
                returnList.Add(nodePaths[nodePathIndex][i]);
            }
            return returnList;
        }
    }
}

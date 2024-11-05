using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    public class ChunkPortData
    {
        private Dictionary<Orientation, List<ComponentPort>> portsDict;

        public ChunkPortData()
        {
            portsDict = new();
        }

        #region Register Ports
        public void RegisterPorts(params ComponentPort[] ports)
        {
            foreach (ComponentPort port in ports)
                RegisterPort(port);
        }

        private void RegisterPort(ComponentPort port)
        {
            if (!portsDict.ContainsKey(port.orientation))
                portsDict.Add(port.orientation, new());

            portsDict[port.orientation].Add(port);
        }
        #endregion

        #region Get Closest Port
        public bool TryFindClosestPort(
            Vector2Int searchPos,
            Orientation desiredOrientation,
            out ComponentPort port)
        {
            port = default;

            if (!portsDict.ContainsKey(desiredOrientation))
                return false;

            List<ComponentPort> validPorts = GetValidComponentPorts(searchPos, desiredOrientation);

            if (validPorts.Count == 0)
                return false;

            port = FindClosestPort(validPorts, searchPos);
            return true;
        }

        private List<ComponentPort> GetValidComponentPorts(
            Vector2Int searchPos,
            Orientation desiredOrientation)
        {
            List<ComponentPort> validPorts = new();

            foreach (ComponentPort port in portsDict[desiredOrientation])
                if (IsValidPort(port, searchPos, desiredOrientation))
                    validPorts.Add(port);

            return validPorts;
        }

        private bool IsValidPort(
            ComponentPort port,
            Vector2Int searchPos,
            Orientation desiredOrientation)
        {
            Vector2Int portPos = port.position;

            return desiredOrientation switch
            {
                Orientation.Up => IsPortBelow(portPos, searchPos),
                Orientation.Right => IsPortLeft(portPos, searchPos),
                Orientation.Down => IsPortAbove(portPos, searchPos),
                Orientation.Left => IsPortRight(portPos, searchPos),
                _ => false,
            };
        }

        #region position Checks
        private bool IsPortBelow(Vector2Int portPos, Vector2Int searchPos)
        {
            return portPos.x == searchPos.x &&
                portPos.y < searchPos.y;
        }

        private bool IsPortLeft(Vector2Int portPos, Vector2Int searchPos)
        {
            return portPos.x < searchPos.x &&
                portPos.y == searchPos.y;
        }

        private bool IsPortAbove(Vector2Int portPos, Vector2Int searchPos)
        {
            return portPos.x == searchPos.x &&
                portPos.y > searchPos.y;
        }

        private bool IsPortRight(Vector2Int portPos, Vector2Int searchPos)
        {
            return portPos.x > searchPos.x &&
                portPos.y == searchPos.y;
        }
        #endregion

        private ComponentPort FindClosestPort(List<ComponentPort> ports, Vector2Int searchPos)
        {
            float closestDistance = DistanceToPort(ports[0], searchPos);
            ComponentPort closestPort = ports[0];

            for (int i = 1; i < ports.Count; i++)
            {
                float dist = DistanceToPort(ports[i], searchPos);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestPort = ports[i];
                }
            }

            return closestPort;
        }

        private float DistanceToPort(ComponentPort port, Vector2Int searchPos)
        {
            return (port.position - searchPos).magnitude;
        }
        #endregion
    }
}

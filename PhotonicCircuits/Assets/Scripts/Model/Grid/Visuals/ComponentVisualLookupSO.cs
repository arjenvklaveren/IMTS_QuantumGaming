using Game.Data;
using SadUtils;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ComponentVisualsLookup", menuName = "ScriptableObjects/Grid/Visuals Lookup")]
    public class ComponentVisualLookupSO : ScriptableObject
    {
        [SerializeField] private UnityDictionary<OpticComponentType, ComponentVisuals> visualsLookup;

        public ComponentVisuals GetPrefabByComponentType(OpticComponentType type)
        {
            if (!visualsLookup.ContainsKey(type))
                return null;

            return visualsLookup[type];
        }
    }
}

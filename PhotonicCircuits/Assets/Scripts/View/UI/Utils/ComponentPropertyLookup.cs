using Game.Data;
using Game.UI;
using SadUtils;
using System.Collections;
using UnityEngine;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "ComponentPropertyLookup", menuName = "ScriptableObjects/UI/Property Lookup")]
    public class ComponentPropertyLookup : ScriptableObject
    {
        [SerializeField] private UnityDictionary<ComponentPropertyType, ComponentPropertyContext> attributeLookup;

        public ComponentPropertyContext GetPrefabByComponentType(ComponentPropertyType type)
        {
            if (!attributeLookup.ContainsKey(type))
                return null;

            return attributeLookup[type];
        }
    }
}

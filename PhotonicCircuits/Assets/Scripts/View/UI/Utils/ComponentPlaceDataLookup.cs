using Game.Data;
using SadUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "ComponentPlaceDataLookup", menuName = "ScriptableObjects/UI/PlaceData Lookup")]
    public class ComponentPlaceDataLookup : ScriptableObject
    {
        [SerializeField] private UnityDictionary<OpticComponentType, ComponentPlaceDataSO> placeDataLookup;

        public ComponentPlaceDataSO GetPlaceDataByType(OpticComponentType type)
        {
            if (!placeDataLookup.ContainsKey(type))
                return null;

            return placeDataLookup[type];
        }
    }
}

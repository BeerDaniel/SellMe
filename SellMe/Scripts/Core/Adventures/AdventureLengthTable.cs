using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Adventures
{
    [System.Serializable, CreateAssetMenu(fileName = "AdventureLengthTable", menuName = "ScriptableObjects/Adventures/AdventureLengthTable")]
    public class AdventureLengthTable : ScriptableObject
    {
        public List<AdventureLengthStepWrapper> list;
    }
}


using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Adventures
{
    [System.Serializable, CreateAssetMenu(fileName = "Adventure", menuName = "ScriptableObjects/Adventures/Adventure")]
    public class Adventure : ScriptableObject
    {
        [SerializeField]
        private AdventureData _adventureData;

        public AdventureData AdventureData => _adventureData;
    }
}


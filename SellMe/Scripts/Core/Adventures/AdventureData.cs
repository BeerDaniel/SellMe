using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Adventures
{
    [System.Serializable]
    public class AdventureData
    {
        #region Exposed private fields
        [SerializeField]
        private string _name = "Default name";
        [SerializeField, TextArea(3, 5)]
        private string _decsription = "Default Description";
        [SerializeField]
        private AdventureLengthStepWrapper _lengthData;
        [SerializeField]
        private bool _specialAdventure = false;
        [SerializeField]
        private int _usesLeft = 1;
        /*
        [SerializeField, HideInInspector]
        private Sprite _sprite;       
        [SerializeField, HideInInspector]       
        private AssetSet _assetSet;
        [SerializeField, HideInInspector]
        private Layout _layout;
        */
        #endregion

        #region Private fields
        [SerializeField, HideInInspector]
        private int _goldCost;
        [SerializeField, HideInInspector]
        private int _timeCost;
        #endregion

        #region Properties
        public string Name => _name;
        public string Decsription => _decsription;
        public int GoldCost => _goldCost;
        public int TimeCost => _timeCost;
        public int AdventureMaxSteps => _lengthData.maxSteps;
        public AdventureLengthStepWrapper Length => _lengthData;
        public bool SpecialAdventure => _specialAdventure;
        /*
        public Sprite Sprite => _sprite;
        public AssetSet => _assetSet;
        public Layout => _layout;
         */
        #endregion

        public AdventureData(string name, AdventureLengthStepWrapper lengthData)
        {
            _name = name;
            _lengthData = lengthData;
            //TODO: cost calculation for adventure
            _goldCost = 1337;
            _timeCost = 1337;
        }
    }
}


using SuspiciousGames.SellMe.Core.Items;
using SuspiciousGames.SellMe.Utility.Sensors;
using System.Collections.Generic;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Adventures
{
    [System.Serializable]
    public class ActiveAdventure
    {
        [SerializeField]
        private AdventureData _adventureData;
        [SerializeField]
        private int _lastStepCountRead;
        [SerializeField]
        private int _walkedSteps;
        [SerializeField, HideInInspector]
        private List<Item> _generatedItems;

        [HideInInspector]
        public int maxItemRolls;
        [HideInInspector]
        public int timesRolled = 0;


        public AdventureData AdventureData { get => _adventureData; }
        public List<Item> GeneratedItems => _generatedItems;
        public int LastStepCountRead => _lastStepCountRead;
        public int WalkedSteps => _walkedSteps;
        public int SurplusSteps => _walkedSteps >= _adventureData.Length.maxSteps ? _walkedSteps - _adventureData.Length.maxSteps : 0;
        public string UiName { get; private set; }

        public ActiveAdventure(AdventureData adventureData)
        {
            _adventureData = adventureData;
            _lastStepCountRead = StepSensorReader.GetStepCount();
            _walkedSteps = 0;
            UiName = $"Adventure - {_adventureData.Length.length.ToString()} - {_adventureData.Name}";
            _generatedItems = new List<Item>();
        }

        public void UpdateWalkedSteps(int walkedsteps)
        {
            _walkedSteps += walkedsteps;
            _lastStepCountRead += walkedsteps;
        }

        public void UpdateLastStepCountRead(int lastStepCountRead)
        {
            _lastStepCountRead = lastStepCountRead;
        }
    }
}


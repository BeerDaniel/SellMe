using SuspiciousGames.SellMe.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Adventures
{
    [System.Serializable, CreateAssetMenu(fileName = "AdventureGenerator", menuName = "ScriptableObjects/AdventureGenerator")]
    public class AdventureGenerator : ScriptableObject
    {
        #region Singleton
        private static AdventureGenerator _instance;

        public static AdventureGenerator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Resources.Load<AdventureGenerator>("AdventureGenerator");
                return _instance;
            }
        }
        #endregion

        public AdventureLengthTable adventureLengthTable;

        [SerializeField]
        private int _maxAdventures;
        [SerializeField]
        private List<Adventure> _specialAdventures;

        [SerializeField, HideInInspector]
        private List<AdventureData> _adventureDatas;

        public bool Generate(out AdventureData result)
        {
            int aLTCount = adventureLengthTable.list.Count;
            if (aLTCount == 0)
            {
                Debug.Log("No Adventure Generated");
                result = null;
                return false;
            }
            else
            {
                var aLSW = GetRandomEntry(adventureLengthTable.list);

                result = new AdventureData("Test Adventure: " + aLSW.length, aLSW);
                return true;
            }
        }

        public List<AdventureData> UpdateAdventureList()
        {
            int missingAdventures = _maxAdventures;
            _adventureDatas = new List<AdventureData>();

            //Load Predefined
            for (int i = 0; i < _specialAdventures.Count; i++)
            {
                if (!Equals(null, _specialAdventures[i]) && missingAdventures > 0)
                {
                    var temp = _specialAdventures[i].AdventureData;
                    _adventureDatas.Add(temp);
                    missingAdventures--;
                }
            }

            //Reload Adventures from Save
            SaveGame.LoadData(SaveId.AdventureList, out string json);
            List<AdventureData> reloadedAdventures;
            if (json != null)
                reloadedAdventures = new List<AdventureData>(JsonHelper.FromJsonArray<AdventureData>(json));
            else
                reloadedAdventures = new List<AdventureData>();

            for (int i = 0; i < reloadedAdventures.Count; i++)
            {
                if (!Equals(null, reloadedAdventures[i]) && missingAdventures > 0)
                {
                    var temp = reloadedAdventures[i];
                    if (temp.SpecialAdventure)
                        continue;
                    _adventureDatas.Add(temp);
                    missingAdventures--;
                }
            }

            //Fill List with Generated Adventures
            for (int i = 0; i < missingAdventures; i++)
            {
                Generate(out AdventureData temp);
                _adventureDatas.Add(temp);
            }

            _adventureDatas = _adventureDatas.OrderBy(a => a.Length.length).ThenBy(a => a.Length.maxSteps).ToList();

            SaveGame.SaveData(SaveId.AdventureList, _adventureDatas);
            return _adventureDatas;
        }

        private T GetRandomEntry<T>(List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
    }
}


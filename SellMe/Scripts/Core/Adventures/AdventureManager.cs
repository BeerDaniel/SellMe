using SuspiciousGames.SellMe.Core.Generators;
using SuspiciousGames.SellMe.Core.Items;
using SuspiciousGames.SellMe.Core.UI;
using SuspiciousGames.SellMe.GameEvents;
using SuspiciousGames.SellMe.Utility;
using SuspiciousGames.SellMe.Utility.Sensors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SuspiciousGames.SellMe.Core.Adventures
{
    public class AdventureManager : MonoBehaviour
    {
        #region Singleton
        public static AdventureManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        #region Exposed private fields
        [SerializeField]
        private int _stepsForRoll;
        [SerializeField]
        private PlayMakerFSM _guiFSM;
        [SerializeField]
        private GameObject _summaryItemPrefab;
        [SerializeField]
        private ItemCard _bestItemCard;
        [SerializeField]
        private TextMeshProUGUI _numOfFoundItemsText;
        [SerializeField]
        private TextMeshProUGUI _surplusStepsText;
        [SerializeField]
        private TextMeshProUGUI _numOfCommonItemsText;
        [SerializeField]
        private TextMeshProUGUI _numOfRareItemsText;
        [SerializeField]
        private TextMeshProUGUI _numOfEpicItemsText;
        [SerializeField]
        private TextMeshProUGUI _numOfLegendaryItemsText;
        [SerializeField]
        private GameObject _summaryItemListObject;
        #endregion

        #region Private fields
        private bool _initiated = false;
        private ActiveAdventure _activeAdventure;
        private Coroutine _stepRoutine;
        private bool _checkForSteps = false;
        #endregion

        #region Properties
        public ActiveAdventure ActiveAventure { get => _activeAdventure; private set => _activeAdventure = value; }
        public Item BestItem { get; private set; }
        #endregion

        #region MonoBehaviour
        private void Start()
        {
            GameManager.Instance.SwitchToGameState(GameState.Adventure);
            SaveGame.LoadData(SaveId.ActiveAdventure, out string json);
            if (BestItem == null)
                _bestItemCard.gameObject.SetActive(false);
            _activeAdventure = JsonUtility.FromJson<ActiveAdventure>(json);
            _activeAdventure.UpdateLastStepCountRead(StepSensorReader.GetStepCount());
            _activeAdventure.maxItemRolls = _activeAdventure.AdventureData.Length.maxSteps / _stepsForRoll;
            _initiated = true;
            if (_stepRoutine != null)
            {
                StopAllCoroutines();
                _stepRoutine = null;
            }
            _checkForSteps = true;
            _stepRoutine = StartCoroutine(StepRoutine());
        }

        private void Update()
        {
            if (_stepRoutine == null)
                StartCoroutine(StepRoutine());
        }

        private void OnEnable()
        {
            if (_initiated)
            {
                if (_stepRoutine != null)
                {
                    StopAllCoroutines();
                    _stepRoutine = null;
                }
                _checkForSteps = true;
                _stepRoutine = StartCoroutine(StepRoutine());
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _checkForSteps = false;
            _stepRoutine = null;
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                StopAllCoroutines();
                _stepRoutine = null;
                _checkForSteps = false;
            }
            else
            {
                _checkForSteps = true;
                SaveGame.LoadData(SaveId.ActiveAdventure, out string data);
                JsonUtility.FromJsonOverwrite(data, _activeAdventure);
                StartCoroutine(StepRoutine());
                //TODO: Check for reboot or other not accounted steps from saved variables
            }
        }
        #endregion

        #region Private methods
        private IEnumerator StepRoutine()
        {
            do
            {
                //Read last stepcount from datasource
                int stepcount = StepSensorReader.GetStepCount();
                _activeAdventure.UpdateWalkedSteps(stepcount - _activeAdventure.LastStepCountRead);

                var amountToRoll = Mathf.Clamp(_activeAdventure.WalkedSteps / _stepsForRoll, 0, _activeAdventure.maxItemRolls) - _activeAdventure.timesRolled;
                for (int i = 0; i < amountToRoll; i++)
                {
                    if (ItemGenerator.Instance.Generate(out Item item))
                    {
                        _activeAdventure.GeneratedItems.Add(item);
                        EventManager.TriggerEvent(GameEventID.ItemGenerated, item);
                    }
                    _activeAdventure.timesRolled++;
                    EventManager.TriggerEvent(GameEventID.ItemRoll, item);
                    if (_activeAdventure.timesRolled == _activeAdventure.maxItemRolls)
                    {
                        _checkForSteps = false;
                        break;
                    }
                }
                _guiFSM.SendEvent("updateUi");
                SaveGame.SaveData(SaveId.ActiveAdventure, _activeAdventure);
                yield return new WaitForSecondsRealtime(2f);
            } while (_checkForSteps);
            EventManager.TriggerEvent(GameEventID.AdventureFinished);
        }
        #endregion

        #region Public methods
        public void EndAventure()
        {
            StopAllCoroutines();
            _checkForSteps = false;
            List<Item> sortedList = _activeAdventure.GeneratedItems.OrderByDescending(a => a.Rarity).ThenBy(a => a.Value).ToList();
            if (sortedList.Count > 0)
            {
                _bestItemCard.gameObject.SetActive(true);
                BestItem = sortedList[0];
                BestItem.isRevealed = true;
                _bestItemCard.Init(BestItem);
            }
            else
            {
                _bestItemCard.gameObject.SetActive(false);
            }
            int numOfCommon = 0;
            int numOfRare = 0;
            int numOfEpic = 0;
            int numOfLegendary = 0;

            foreach (var item in sortedList)
            {
                item.isRevealed = true;
                switch (item.Rarity)
                {
                    case Rarity.Common:
                        numOfCommon++;
                        break;
                    case Rarity.Rare:
                        numOfRare++;
                        break;
                    case Rarity.Epic:
                        numOfEpic++;
                        break;
                    case Rarity.Legendary:
                        numOfLegendary++;
                        break;
                    default:
                        break;
                }
                var summaryItem = Instantiate(_summaryItemPrefab).GetComponent<SummaryItem>();
                summaryItem.transform.SetParent(_summaryItemListObject.transform);
                summaryItem.Init(item);
            }
            _numOfFoundItemsText.text = sortedList.Count.ToString();
            _surplusStepsText.text = _activeAdventure.SurplusSteps.ToString();
            _numOfCommonItemsText.text = numOfCommon.ToString();
            _numOfRareItemsText.text = numOfRare.ToString();
            _numOfEpicItemsText.text = numOfEpic.ToString();
            _numOfLegendaryItemsText.text = numOfLegendary.ToString();

            var player = Player.Instance;

            foreach (var item in _activeAdventure.GeneratedItems)
            {
                if (!player.Inventory.AddItem(item))
                    break;
            }
            player.Save();
            GameManager.Instance.SwitchToGameState(GameState.Default);
            SaveGame.SaveData(SaveId.ActiveAdventure, "");
        }
        #endregion
    }
}
using SuspiciousGames.SellMe.Core.Adventures;
using SuspiciousGames.SellMe.Core.Items;
using SuspiciousGames.SellMe.GameEvents;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace SuspiciousGames.SellMe.Core.UI
{
    public class OutsideUiManager : MonoBehaviour
    {
        #region Private exposed fields
        [SerializeField]
        private GameObject _itemCardPrefab;
        [SerializeField]
        private HorizontalScrollSnap _itemScrollSnap;
        [SerializeField]
        private PlayMakerFSM _playMakerFSM;
        #endregion

        #region Private fields
        private List<ItemCard> _itemCards;
        #endregion

        #region MonoBehaviour
        private void Start()
        {
            if (_itemCards == null)
            {
                _itemCards = new List<ItemCard>();
                foreach (var item in AdventureManager.Instance.ActiveAventure.GeneratedItems)
                {
                    var itemCard = Instantiate(_itemCardPrefab).GetComponent<ItemCard>();
                    itemCard.Init(item);
                    _itemCards.Add(itemCard);
                    _itemScrollSnap.AddChild(itemCard.gameObject);
                }
            }
        }

        private void OnEnable()
        {
            EventManager.Subscribe(GameEventID.AdventureFinished, OnAdventureFinished);
            EventManager.Subscribe(GameEventID.ItemGenerated, OnItemGenerated);
            EventManager.Subscribe(GameEventID.ItemRoll, OnItemRoll);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(GameEventID.AdventureFinished, OnAdventureFinished);
            EventManager.Unsubscribe(GameEventID.ItemGenerated, OnItemGenerated);
            EventManager.Unsubscribe(GameEventID.ItemRoll, OnItemRoll);
        }
        #endregion

        #region Public methods
        public void SetUpSummary()
        {
            //TODO: Summary UI code
        }

        public void UpdateCardList()
        {
            if (!_itemScrollSnap.ChildObjects.Equals(_itemCards))
            {
                List<GameObject> children = new List<GameObject>(_itemScrollSnap.ChildObjects);
                foreach (var itemCard in _itemCards)
                {
                    if (children.Contains(itemCard.gameObject))
                        continue;
                    _itemScrollSnap.AddChild(itemCard.gameObject);
                }
            }
        }
        #endregion

        #region Callbacks
        private void OnAdventureFinished(GameEvent arg0)
        {
            _playMakerFSM.SendEvent("complete");
        }

        private void OnItemGenerated(GameEvent arg0)
        {
            var itemCard = Instantiate(_itemCardPrefab).GetComponent<ItemCard>();
            itemCard.Init(arg0.Data[0] as Item);
            _itemCards.Add(itemCard);
            _playMakerFSM.SendEvent("updateUi");
        }

        private void OnItemRoll(GameEvent arg0)
        {
            _playMakerFSM.SendEvent("updateUi");
        }
        #endregion
    }
}
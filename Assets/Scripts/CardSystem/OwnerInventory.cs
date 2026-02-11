using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using DG.Tweening;
using FishONU.CardSystem.CardArrangeStrategy;
using Mirror;
using Mirror.Examples.Common.Controllers.Tank;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FishONU.CardSystem
{
    [System.Serializable]
    public class OwnerInventory : BaseInventory
    {
        private Dictionary<string, GameObject> localCardObjs = new();

        public readonly List<CardData> LocalCards = new();
        public readonly SyncList<CardData> Cards = new();

        public int LocalCardNumber => LocalCards.Count;

        private CardData _highLightCard;

        // TODO: Highlight 在多人游玩的时候 remote client 报错，需要修
        public CardData HighlightCard
        {
            get => _highLightCard;
            set
            {
                if (value == null)
                {
                    Debug.LogError("HighlightCardObj is not CardObj");
                    return;
                }

                if (!localCardObjs.ContainsKey(value.guid))
                {
                    Debug.LogError("HighlightCard is not in localCardObjs");
                    return;
                }

                SetHighlightCard(value);
            }
        }

        #region View

        public override IArrangeStrategy GetDefaultArrangeStrategy()
        {
            return new CenterLinearWithArc
            {
                CenterPosition = cardSpawnPosition,
                PositionOffset = new(0.65f, 0.1f, 0f),
                RotationOffset = new(0f, 0f, -5f)
            };
        }

        [Client]
        public void ClientAddCard(CardData cardData)
        {
            if (cardData == null) return;

            LocalCards.Add(cardData);

            RefreshView();
        }

        [Client]
        public void ClientRemoveCard(CardData cardData)
        {
            if (LocalCards.Count == 0) return;

            foreach (var c in LocalCards)
            {
                if (c.Guid != cardData.Guid) continue;

                LocalCards.Remove(c);
                break;
            }

            RefreshView();
        }

        [Client]
        private void SortAllCards()
        {
            LocalCards.Sort((a, b) =>
            {
                var colorCmp = a.color.CompareTo(b.color);
                if (colorCmp != 0) return colorCmp;

                var faceCmp = a.face.CompareTo(b.face);
                if (faceCmp != 0) return faceCmp;

                return String.Compare(a.Guid, b.Guid, StringComparison.Ordinal);
            });
        }


        /// <summary>
        /// 重新加载数据到视图
        /// 
        /// 疑似 bug: 不要和 SetHighlightCard 在同一帧使用，因为 RefreshView 会覆盖动画
        /// 正常情况应该不会遇到，但是预防一下
        /// </summary>
        [Client]
        public override void RefreshView()
        {
            base.RefreshView();

            SetHighlightCard(_highLightCard);
        }

        [Client]
        public override void ArrangeAllCards()
        {
            if (LocalCards.Count == 0) return;

            SortAllCards();

            for (var i = 0; i < LocalCards.Count; i++)
            {
                var guid = LocalCards[i].Guid;
                if (localCardObjs.TryGetValue(guid, out var obj))
                {
                    var t = obj.transform;
                    ArrangeStrategy.Calc(i, LocalCards.Count, out var pos, out var rotation, out var scale);
                    t.DOKill();
                    t.transform.DOLocalMove(pos, 0.5f).SetEase(Ease.InOutQuad);
                    t.transform.DOLocalRotate(rotation, 0.5f).SetEase(Ease.InOutQuad);
                    t.transform.DOScale(scale, 0.5f).SetEase(Ease.InOutQuad);
                }
            }
        }


        [Client]
        public override void InstantiateAllCards()
        {
            // instantiate new cards
            foreach (var card in LocalCards)
            {
                if (localCardObjs.ContainsKey(card.Guid)) continue;

                var cardObj = Instantiate(cardPrefab, gameObject.transform);
                cardObj.transform.localPosition = cardSpawnPosition;
                cardObj.GetComponent<CardObj>().Load(card);
                localCardObjs.Add(card.Guid, cardObj);
            }

            // clean non-exist card
            var cardGuidSet = new HashSet<string>(LocalCards.Select(c => c.Guid));
            var toRemove = new List<string>();
            foreach (var pair in localCardObjs)
            {
                if (cardGuidSet.Contains(pair.Key)) continue;

                toRemove.Add(pair.Key);
            }

            foreach (var guid in toRemove)
            {
                var obj = localCardObjs[guid];

                if (obj.TryGetComponent<CardObj>(out var card))
                {
                    card.FadeOutAndDestroy();
                }
                else
                {
                    Destroy(obj);
                }


                localCardObjs.Remove(guid);
            }
        }


        [Client]
        private void ResetHighlightCard()
        {
            if (_highLightCard == null) return;

            Debug.Log("ResetHighlightCard");

            var card = _highLightCard;
            _highLightCard = null;


            var index = LocalCards.FindIndex(c => c.guid == card.guid);
            if (index == -1)
                return;


            var t = localCardObjs[card.guid].transform;

            _highLightCard = null;

            ArrangeStrategy.Calc(index, LocalCards.Count, out Vector3 pos, out var rot, out var scale);

            t.DOKill();
            t.DOLocalMove(pos, 0.2f).SetEase(Ease.InOutQuad);
            t.DOLocalRotate(rot, 0.2f).SetEase(Ease.InOutQuad);
            t.DOScale(scale, 0.2f).SetEase(Ease.InOutQuad);
        }


        [Client]
        private void SetHighlightCard(CardData cardData = null)
        {
            if (cardData == null) return;

            Debug.Log($"SetHighlightCard: {cardData.guid}");

            // set the animation sign

            if (_highLightCard != null && _highLightCard.guid == cardData.guid)
            {
                // 取消高光因为点了一张卡两次
                ResetHighlightCard();
                return;
            }

            // reset old highlight card.
            // 不需要设置动画状态，因为上面已经保证了重置动画的卡片和接下来要进行动画的卡片是不一样的
            ResetHighlightCard();

            _highLightCard = cardData;

            // highlight new card
            var index = LocalCards.FindIndex(c => c.guid == cardData.guid);
            if (index == -1) return;

            // highlight animation
            var t = localCardObjs[cardData.guid].transform;

            t.DOKill();
            t.DOLocalMove(t.localPosition + new Vector3(0, 0.2f, 0), 0.2f)
                .SetEase(Ease.InOutQuad);
            t.DOScale(1.2f, 0.2f).SetEase(Ease.InOutQuad);
        }

        #endregion

        #region Network

        public override void OnStartClient()
        {
            // 如果是 host 模式，防止显示其他人的手牌
            if (!isLocalPlayer) return;

            Cards.Callback += OnSyncCardChange;

            RefreshView();
        }

        public override void OnStopClient()
        {
            // 如果是 host 模式，防止显示其他人的手牌
            if (!isLocalPlayer) return;

            Cards.Callback -= OnSyncCardChange;
        }

        [Client]
        private void OnSyncCardChange(SyncList<CardData>.Operation op, int index, CardData oldItem, CardData newItem)
        {
            LocalCards.Clear();
            LocalCards.AddRange(Cards);

            // TODO: 实现增量更新
            InstantiateAllCards();
            ArrangeAllCards();
        }

        [Server]
        public void PlayCard(CardData card)
        {
            // TODO: play card
            Debug.Log($"play card: face: {card.face.ToString()}; color: {card.color.ToString()}");
            LocalCards.Remove(card);
        }

        #endregion

        #region Debug

        [Command]
        public void DebugCmdAddCard()
        {
            var cardInfo = CardDataFactory.CreateRandomCard();

            Cards.Add(cardInfo);
        }

        [Command]
        public void DebugCmdRemoveCard()
        {
            if (Cards.Count == 0) return;

            Cards.RemoveAt(Random.Range(0, Cards.Count));
        }

        #endregion
    }
}
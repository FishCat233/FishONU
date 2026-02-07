using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Mirror;
using UnityEngine;

namespace FishONU.CardSystem
{
    [System.Serializable]
    public class OwnerInventory : NetworkBehaviour
    {
        [SerializeField] private GameObject cardPrefab;

        private Dictionary<string, GameObject> cardObjs = new();
        public readonly List<CardInfo> cards = new();
        private readonly SyncList<CardInfo> _syncCards = new();

        public enum ArrangeType
        {
            HorizontalCenter, // 横向均匀居中（手牌专用，推荐）
            StackOffset, // 叠放偏移（弃牌堆专用，顶牌在正位，下面的牌偏移）
            StackOverlap // 完全堆叠（牌库专用，所有牌叠在一起）
        }

        [Header("卡牌排版配置")] public ArrangeType cardArrangeType;
        public float cardWidth = 1.3f;
        public float cardHeight = 1.9f;
        public Vector2 stackOffset = new Vector2(0.15f, -0.15f); // 叠放偏移
        public float smoothMoveTime = 0.2f; // 卡牌平滑移动时间（0则瞬移）
        public Vector3 cardSpawnPos;

        private void Start()
        {
            if (cardPrefab == null) Debug.LogError("CardPrefab is null");
            if (cardSpawnPos == Vector3.zero)
                cardSpawnPos = gameObject.transform.position;
        }

        private void OnEnable()
        {
            if (isClient)
            {
                _syncCards.Callback += OnSyncCardChange;
            }
        }

        private void OnDisable()
        {
            if (isClient)
            {
                _syncCards.Callback -= OnSyncCardChange;
            }
        }


        [Client]
        public void DebugAddCard(CardInfo cardInfo)
        {
            cardInfo ??= new CardInfo();

            cards.Add(cardInfo);

            InstantiateAllCard();
            ArrangeAllCard();
        }

        [Client]
        public void DebugRemoveCard(CardInfo cardInfo = null)
        {
            if (cards.Count == 0) return;

            // 随机删牌
            if (cardInfo == null)
            {
                var count = cards.Count;
                var index = Random.Range(0, count);
                cardInfo = cards[index];
            }

            cards.Remove(cardInfo);

            InstantiateAllCard();
            ArrangeAllCard();
        }

        [ClientRpc]
        public void RpcAddCard(CardInfo cardInfo)
        {
            // TODO:
            InstantiateAllCard();
            ArrangeAllCard();
        }

        [ClientRpc]
        public void RpcRemoveCard(CardInfo cardInfo)
        {
            // TODO:
            InstantiateAllCard();
            ArrangeAllCard();
        }

        [Client]
        private void OnSyncCardChange(SyncList<CardInfo>.Operation op, int index, CardInfo oldItem, CardInfo newItem)
        {
            cards.Clear();
            cards.AddRange(_syncCards);

            // TODO: 实现增量更新
            InstantiateAllCard();
            ArrangeAllCard();
        }

        [Server]
        public void PlayCard(CardInfo card)
        {
            // TODO: play card
            Debug.Log($"play card: face: {card.face.ToString()}; color: {card.color.ToString()}");
            cards.Remove(card);
        }

        [Client]
        public void ArrangeAllCard()
        {
            // TODO: arrange all card
            if (cards.Count == 0) return;

            SortAllCard();

            switch (cardArrangeType)
            {
                case ArrangeType.HorizontalCenter:
                    ArrangeHorizontalCenter();
                    break;
                case ArrangeType.StackOffset:
                case ArrangeType.StackOverlap:
                    // TODO:
                    break;
            }
        }

        [Client]
        private void SortAllCard()
        {
            cards.Sort((a, b) =>
            {
                var colorCmp = a.color.CompareTo(b.color);
                if (colorCmp != 0) return colorCmp;

                return a.face.CompareTo(b.face);
            });
        }

        [Client]
        public void InstantiateAllCard()
        {
            foreach (var card in cards)
            {
                if (cardObjs.ContainsKey(card.Guid)) continue;

                var cardObj = Instantiate(cardPrefab, gameObject.transform);
                cardObj.transform.position = cardSpawnPos;
                cardObj.GetComponent<CardObj>().Load(card);
                cardObjs.Add(card.Guid, cardObj);
            }

            // clean not exist card
            var cardGuidSet = new HashSet<string>(cards.Select(c => c.Guid));
            var toRemove = new List<string>();
            foreach (var pair in cardObjs)
            {
                if (cardGuidSet.Contains(pair.Key)) continue;

                toRemove.Add(pair.Key);
            }

            foreach (var guid in toRemove)
            {
                // TODO: add more animation
                // Destroy(cardObjs[guid]);
                var obj = cardObjs[guid];
                if (obj.TryGetComponent<SpriteRenderer>(out var sp))
                {
                    sp.DOFade(0, 0.5f).OnComplete(() => { Destroy(obj); });
                }
                else
                {
                    Destroy(obj);
                }

                cardObjs.Remove(guid);
            }
        }

        [Client]
        private void ArrangeHorizontalCenter()
        {
            // TODO: arrange card horizontally center
            for (var i = 0; i < cards.Count; i++)
            {
                var guid = cards[i].Guid;
                if (cardObjs.TryGetValue(guid, out var obj))
                {
                    var t = obj.transform;
                    var targetPos = new Vector3(i * cardWidth, 0, 0);
                    t.DOKill();
                    t.transform.DOMove(targetPos, 0.5f).SetEase(Ease.InOutQuad);
                }
            }
        }
    }
}
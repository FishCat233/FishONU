using System.Collections.Generic;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

namespace FishONU.CardSystem
{
    [System.Serializable]
    public class CardInventory : NetworkBehaviour
    {
        [SerializeField] private GameObject cardPrefab;
        private Dictionary<string, GameObject> cardGo;
        public readonly SyncList<CardInfo> cards = new();

        public enum ArrangeType
        {
            HorizontalCenter, // 横向均匀居中（手牌专用，推荐）
            StackOffset, // 叠放偏移（弃牌堆专用，顶牌在正位，下面的牌偏移）
            StackOverlap // 完全堆叠（牌库专用，所有牌叠在一起）
        }

        [Header("卡牌排版配置")] public ArrangeType cardArrangeType;
        public float cardSpacing = 1.2f; // 间距
        public Vector2 stackOffset = new Vector2(0.15f, -0.15f); // 叠放偏移
        public float smoothMoveTime = 0.2f; // 卡牌平滑移动时间（0则瞬移）

        [Client]
        public void DebugAddCard(CardInfo cardInfo)
        {
            cards.Add(cardInfo);
        }

        [Client]
        public void DebugRemoveCard(CardInfo cardInfo = null)
        {
            if (cards.Count == 0) return;

            cardInfo ??= cards[0];
            cards.Remove(cardInfo);
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
        private void ArrangeHorizontalCenter()
        {
            // TODO: arrange card horizontally center
        }

        private void OnCardListChanged(List<CardInfo> old, List<CardInfo> cur)
        {
            ArrangeAllCard();
        }
    }
}
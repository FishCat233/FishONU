using System;
using System.Collections.Generic;
using FishONU.CardSystem;
using Mirror;
using UnityEngine;

namespace FishONU.PlayerController
{
    public class CardController : NetworkBehaviour
    {
        [SerializeField] private CardInventory cardInventory;

        private void Start()
        {
            // 不用 DI 也行，简单来，反正在 Start 初始化
            if (cardInventory == null)
            {
                cardInventory = gameObject.GetComponent<CardInventory>();
                if (cardInventory == null)
                    Debug.LogError("CardInventory is null");
            }
        }

        [Command]
        private void CmdTryPlayCard(CardInfo card)
        {
            ValidateAndPlayCard(card);
        }

        [Server]
        private void ValidateAndPlayCard(CardInfo card)
        {
            var c = cardInventory.cards.Find(c => c.guid == card.guid);
            if (c == null)
            {
                Debug.LogWarning(
                    $"Try to Play a card that is not in the inventory: {card.guid} {card.face.ToString()} {card.color.ToString()}");
                return;
            }

            cardInventory.PlayCard(c);
        }
    }
}
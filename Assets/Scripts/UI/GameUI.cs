using System;
using FishONU.Player;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace FishONU.UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private Button submitCardButton;

        private void Start()
        {
            if (submitCardButton == null) Debug.LogError("SubmitCardButton is null");
            submitCardButton.onClick.AddListener(OnSubmitCardButtonClick);
        }

        private void OnSubmitCardButtonClick()
        {
            if (NetworkClient.localPlayer == null) Debug.LogError("NetworkClient.localPlayer is null");

            NetworkClient.localPlayer.GetComponent<PlayerController>().TryPlayCard();
        }
    }
}
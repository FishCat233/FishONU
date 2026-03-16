using FishONU.CardSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishONU.UI.CardHistory
{

    public class CardHistoryText : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text tmpText;
        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private UnityEngine.Color RedColor;
        [SerializeField]
        private UnityEngine.Color YellowColor;
        [SerializeField]
        private UnityEngine.Color BlueColor;
        [SerializeField]
        private UnityEngine.Color GreenColor;
        [SerializeField]
        private UnityEngine.Color BlackColor;

        private CardData data;
        public CardData Data
        {
            get => data;
            set
            {
                data = value;
                UpdateView();
            }
        }

        private void OnValidate()
        {
            if (tmpText == null) Debug.LogError("CardHistoryText: backgroundImage is null");
            if (backgroundImage == null) Debug.LogError("CardHistoryText: backgroundImage is null");
            if (RedColor == null) Debug.LogError("CardHistoryText: RedColor is null");
            if (YellowColor == null) Debug.LogError("CardHistoryText: YellowColor is null");
            if (BlueColor == null) Debug.LogError("CardHistoryText: BlueColor is null");
            if (GreenColor == null) Debug.LogError("CardHistoryText: GreenColor is null");
            if (BlackColor == null) Debug.LogError("CardHistoryText: BlackColor is null");
        }

        public void UpdateView()
        {
            tmpText.text = DataToString(data);

            backgroundImage.color = DataToColor(data);
        }

        private string DataToString(CardData data)
        {
            return data.face.ToString();
        }

        private UnityEngine.Color DataToColor(CardData data)
        {
            switch (data.color)
            {
                case FishONU.CardSystem.Color.Red:
                    return RedColor;
                case FishONU.CardSystem.Color.Green:
                    return GreenColor;
                case FishONU.CardSystem.Color.Yellow:
                    return YellowColor;
                case FishONU.CardSystem.Color.Blue:
                    return BlueColor;
                case FishONU.CardSystem.Color.Black:
                    return BlackColor;
            }
            return UnityEngine.Color.black;
        }
    }
}

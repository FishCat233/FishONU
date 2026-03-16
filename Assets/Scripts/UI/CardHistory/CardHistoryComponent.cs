using FishONU.CardSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishONU.UI.CardHistory
{


    public class CardHistoryComponent : MonoBehaviour
    {
        [SerializeField]
        private GameObject historyTextPrefab;

        private List<CardHistoryText> historyTexts = new List<CardHistoryText>();

        private List<CardData> data;
        public List<CardData> Data
        {
            get => data;
            set
            {
                data = value;
                UpdateHistory();
            }
        }

        public void UpdateHistory()
        {
            // 清空现有历史文本
            foreach (var text in historyTexts)
            {
                if (text != null)
                {
                    Destroy(text.gameObject);
                }
            }
            historyTexts.Clear();

            // 遍历数据创建新的历史文本
            if (data != null)
            {
                foreach (var cardData in data)
                {
                    var textObj = Instantiate(historyTextPrefab, transform);
                    var historyText = textObj.GetComponent<CardHistoryText>();
                    if (historyText != null)
                    {
                        historyText.Data = cardData;
                        historyTexts.Add(historyText);
                    }
                }
            }
        }
    }
}

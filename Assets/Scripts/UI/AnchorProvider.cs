using System;
using System.Collections.Generic;
using UnityEngine;

namespace FishONU.UI
{
    [Serializable]
    public struct AnchorData
    {
        public string name;
        public Transform anchor;
    }

    public class AnchorProvider : MonoBehaviour
    {
        [SerializeField] public List<Transform> fourSeatsAnchors;

        public static AnchorProvider Instance;

        [SerializeField] public Transform customDrawCardPileAnchor;
        [SerializeField] public Transform customDiscardCardPileAnchor;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            // Check Anchors

            // four seats anchors
            if (fourSeatsAnchors.Count < 4)
                Debug.LogError("AnchorProvider: Not enough anchors for 4 players");
            else
            {
                for (int i = 0; i < 4; i++)
                    if (fourSeatsAnchors[i] == null)
                        Debug.LogError("AnchorProvider: Anchor " + i + " is null");
            }

            if (customDrawCardPileAnchor == null)
                Debug.LogError("AnchorProvider: CustomDrawCardPileAnchor is null");

            if (customDiscardCardPileAnchor == null)
                Debug.LogError("AnchorProvider: CustomDiscardCardPileAnchor is null");
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
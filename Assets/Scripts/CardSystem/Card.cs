using UnityEngine;
using UnityEngine.Serialization;

namespace FishONU.CardSystem
{
    public enum Color
    {
        Red,
        Blue,
        Green,
        Yellow,
        Black
    }

    public enum Face
    {
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Skip,
        Reverse,
        DrawTwo,
        Wild,
        WildDrawFour,
        Back // 背面
    }

    [System.Serializable]
    public class CardInfo
    {
        public Color color;

        public Face face;

        // 卡牌唯一标识符
        [SerializeField] private string guid = "";

        public string Guid
        {
            get => guid;
            set => guid = value;
        }

        public CardInfo()
        {
            this.color = Color.Black;
            this.face = Face.Back;
            Guid = System.Guid.NewGuid().ToString();
        }

        public CardInfo(Color color = Color.Black, Face face = Face.Back)
        {
            this.color = color;
            this.face = face;
            Guid = System.Guid.NewGuid().ToString();
        }
    }
}
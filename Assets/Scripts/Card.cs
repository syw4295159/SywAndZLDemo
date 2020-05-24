using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private CardData Data;

    public Text Text;

    //private void Awake()
    //{
    //    Text.text = Data.CardType == CardType.Move ? "Move" : "Attack";
    //}

    public void SetData(CardData _data)
    {
        Data = _data;
        Text.text = Data.CardType == CardType.Move ? "Move" : "Attack";
    }
}

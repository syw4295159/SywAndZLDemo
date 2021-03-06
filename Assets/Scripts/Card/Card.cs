﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int CardId { get; private set; }

    public CardData Data { get; private set; }

    public Text Text;

    //public bool IsActive;//激活状态，同阶段只有一个card可以激活

    //private void Awake()
    //{
    //    Text.text = Data.CardType == CardType.Move ? "Move" : "Attack";
    //}

    public void SetData(CardData _data)
    {
        CardId = _data.CardId;
        Data = _data;
        Text.text = Data.CardType == CardType.Move ? "Move" : "Attack";
        //IsActive = true;
    }

    public void OnClick()
    {
        if (Data.BelongId != 0) return;
        MessageCenter<int, CardData>.SendMessage((int)MsgEnum.UseCard, 0, Data);
    }

    public void SetClickState(bool state)
    {
        GetComponent<Button>().interactable = state;
    }
}

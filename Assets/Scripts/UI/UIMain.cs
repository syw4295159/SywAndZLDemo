using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIMain : MonoBehaviour
{
    public GameObject StepOne;
    public GameObject StepTime;
    public Text timeText;
    public GameObject StepOver;
    public Animator UIMainAnimator;

    public GameObject CardPrefab;
    public GameObject DownGrid;
    public GameObject UpGrid;

    public List<Card> Cards = new List<Card>();
    private void Awake()
    {
        UIMainAnimator = GetComponent<Animator>();
        StepOne.gameObject.SetActive(false);
        StepTime.gameObject.SetActive(false);
        StepOver.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        MessageCenter.RegisterMessage((int)MsgEnum.StepOne, OnMsgStepOne);
        MessageCenter.RegisterMessage((int)MsgEnum.StepTime, OnMsgStepTime);
        MessageCenter<int, CardData>.RegisterMessage((int)MsgEnum.UseCard, OnMsgUseCardView);
        MessageCenter<int, CardData>.RegisterMessage((int)MsgEnum.BehaviorLimit, OnMsgBehaviorLimit);
    }
    private void OnDisable()
    {
        MessageCenter.UnRegisterMessage((int)MsgEnum.StepOne, OnMsgStepOne);
        MessageCenter.UnRegisterMessage((int)MsgEnum.StepTime, OnMsgStepTime);
        MessageCenter<int, CardData>.UnRegisterMessage((int)MsgEnum.UseCard, OnMsgUseCardView);
        MessageCenter<int, CardData>.UnRegisterMessage((int)MsgEnum.BehaviorLimit, OnMsgBehaviorLimit);
    }

    /// <summary>
    /// 发牌阶段
    /// </summary>
    private void OnMsgStepOne()
    {
        StepOne.gameObject.SetActive(true);
        UIMainAnimator.Play("UIMain_StepOne");
        SpawnCard(CellType.Up);
        SpawnCard(CellType.Down);
    }
    private void SpawnCard(CellType cellType)
    {
        GamePlayer gamePlayer = GameLogic.Instance.PlayerList.Find(x => x.Data.CellType == cellType);
        if (gamePlayer == null) return;
        var playerCardDatas = gamePlayer.Data.CardsList;
        for (int i = 0; i < playerCardDatas.Count; i++)
        {
            var card = Instantiate(CardPrefab, cellType == CellType.Down ? DownGrid.transform : UpGrid.transform);
            var cardAgent = card.GetComponent<Card>() ?? card.AddComponent<Card>();
            cardAgent.SetData(playerCardDatas[i]);
            if(playerCardDatas[i].BelongId == 0)
                Cards.Add(cardAgent);   
        }
    }
    /// <summary>
    /// UIMain_StepOne动画片段结尾事件绑定
    /// </summary>
    private void OnMsgOneOver()
    {        
        MessageCenter.SendMessage((int)MsgEnum.StepOneOver);        
    }
    /// <summary>
    /// 决策阶段，15秒
    /// </summary>
    private void OnMsgStepTime()
    {
        StepOne.gameObject.SetActive(false);
        GameLogic.Instance.GameStartAction += TimeRunning;
        StepTime.gameObject.SetActive(true);
        UIMainAnimator.Play("UIMain_StepTime");
    }
    private void TimeRunning(float timer)
    {
        if (timer >= 15)
        {
            OnMsgStepTimeOver();
        }
        else
        {
            timeText.text = timer >= 0 ? (Convert.ToInt32(15 - timer)).ToString() : "0";
        }        
    }
    private void OnMsgStepTimeOver()
    {
        GameLogic.Instance.GameStartAction -= TimeRunning;
        StepTime.gameObject.SetActive(false);
        StepOver.gameObject.SetActive(true);
        UIMainAnimator.Play("UIMain_StepOver");
        MessageCenter.SendMessage((int)MsgEnum.StepTimeOver);
        Invoke("OnMsgStepOver", 1);
    }
    private void OnMsgStepOver()
    {
        StepOver.gameObject.SetActive(false);
    }

    private void OnMsgUseCardView(int playerId,CardData cardData)
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            if (Cards[i].CardId != cardData.CardId)
            {
                //Cards[i].IsActive = false;
            }
            else
            {
                Debug.LogFormat("现在操作{0}", Cards[i].CardId);
            }
        }
    }

    private void OnMsgBehaviorLimit(int limit,CardData data)
    {
        if (limit <= 0)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].SetClickState(false);
            }
        }
        else
        {
            var card = Cards.Find(x => x.Data == data);
            if(card != null)
            {
                card.SetClickState(false);
            }
        }
    }


}

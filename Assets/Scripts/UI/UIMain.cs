using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

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
    }
    private void OnDisable()
    {
        MessageCenter.UnRegisterMessage((int)MsgEnum.StepOne, OnMsgStepOne);
        MessageCenter.UnRegisterMessage((int)MsgEnum.StepTime, OnMsgStepTime);
    }

    private void OnMsgStepOne()
    {
        StepOne.gameObject.SetActive(true);
        UIMainAnimator.Play("UIMain_StepOne");
        SpawnCard(CellType.Up);
        SpawnCard(CellType.Down);
    }
    private void SpawnCard(CellType cellType)
    {
        GamePlayer DownPlayer = GameLogic.Instance.PlayerList.Find(x => x.Data.CellType == cellType);
        if (DownPlayer == null) return;
        var playerCardDatas = DownPlayer.Data.CardsList;
        for (int i = 0; i < playerCardDatas.Count; i++)
        {
            var card = Instantiate(CardPrefab, cellType == CellType.Down ? DownGrid.transform : UpGrid.transform);
            var cardData = card.GetComponent<Card>() ?? card.AddComponent<Card>();
            cardData.SetData(playerCardDatas[i]);
        }
    }    
    private void OnMsgOneOver()
    {
        MessageCenter.SendMessage((int)MsgEnum.StepOneOver);        
    }
    private void OnMsgStepTime()
    {
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
        MessageCenter.SendMessage((int)MsgEnum.StepTimeOver);
    }
    private void OnMsgStepOver()
    {

    }
}

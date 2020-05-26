using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameLogic : MonoBehaviour
{
    public static GameLogic Instance;

    FsmCore fc;
    [Space]
    [Header("Cell")]
    public GameObject CellSpawner;
    public GameObject GameCellPrefab;
    public int UpCellRow;
    public int UpCellColumn;
    public int DownCellRow;
    public int DownCellColumn;
    private List<GameCell> gameCells = new List<GameCell>();
    private List<GameCell> tempAttackArea = new List<GameCell>();//攻击区域缓存
    [Space]
    [Header("Player")]
    public GameObject GamePlayerPrefab;
    private List<GamePlayer> playerlist = new List<GamePlayer>();
    public List<GamePlayer> PlayerList { get => playerlist; }

    [Space]
    [Header("Logic")]
    private float timer;//决策计时器
    public float overTime = 15;
    private bool isGameDesign = false;
    public Action<float> GameStartAction;
    private CardType currentCardType;//当前正在使用的牌类型
    private CardData currentCardData;
    [Space]
    [Header("Behavior")]     
    public Dictionary<int, List<PlayerControlBehavior>> BehaviorDic = new Dictionary<int, List<PlayerControlBehavior>>();
    private int behaviorIndex = 0;//回合数

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        MessageCenter.RegisterMessage((int)MsgEnum.StepOneOver, OnMsgStepOneOver);
        MessageCenter.RegisterMessage((int)MsgEnum.StepTimeOver, OnMsgStepTimeOver);
        MessageCenter<int, CardData>.RegisterMessage((int)MsgEnum.UseCard, OnMsgUseCard);
        MessageCenter<int, int, CellType>.RegisterMessage((int)MsgEnum.ClickCell, OnMsgClickCell);
    }
    private void Start()
    {
        fc = new FsmCore();
        gameCells.Clear();
        playerlist.Clear();
        tempAttackArea.Clear();
        SpawnCell(UpCellRow, UpCellColumn, CellType.Up);
        SpawnCell(DownCellRow, DownCellColumn, CellType.Down);

        StepOne();
        MessageCenter.SendMessage((int)MsgEnum.StepOne);
    }

    private void OnDisable()
    {
        MessageCenter.UnRegisterMessage((int)MsgEnum.StepOneOver, OnMsgStepOneOver);
        MessageCenter.UnRegisterMessage((int)MsgEnum.StepTimeOver, OnMsgStepTimeOver);
        MessageCenter<int, CardData>.UnRegisterMessage((int)MsgEnum.UseCard, OnMsgUseCard);
        MessageCenter<int, int, CellType>.UnRegisterMessage((int)MsgEnum.ClickCell, OnMsgClickCell);
    }

    private void SpawnCell(int row,int column,CellType type)
    {
        int index = 0;
        var rect = GameCellPrefab.GetComponent<RectTransform>().rect;
        int spe = type == CellType.Up ? 1 : -1;
        int spe2 = type == CellType.Up ? 0 : -100;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                var cell = Instantiate(GameCellPrefab, CellSpawner.transform);
                var gamecell = cell.GetComponent<GameCell>() ?? cell.AddComponent<GameCell>();                
                gamecell.Init(index, type);
                index++;
                gamecell.row = i;
                gamecell.column = j;
                gamecell.SetClickState(false);
                gamecell.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * rect.width, j * rect.height * spe + spe2);                
                gameCells.Add(gamecell);
            }
        }
    }

    /// <summary>
    /// 生成人物，发牌阶段
    /// </summary>
    private void StepOne()
    {
        isGameDesign = false;
        SpawnPlayer(0, 3, PlayerType.Player, CellType.Down);
        SpawnPlayer(1, 3, PlayerType.Enemy, CellType.Up);
    }
    /// <summary>
    /// 发牌结束
    /// </summary>
    private void OnMsgStepOneOver()
    {
        Debug.Log("StepOneOver");
        tempAttackArea.Clear();
        MessageCenter.SendMessage((int)MsgEnum.StepTime);
        isGameDesign = true;
        
    }

    private void OnMsgStepTimeOver()
    {
        Debug.Log("倒计时结束");
        BehaviorActoring(behaviorIndex);
        behaviorIndex++;

    }

    /// <summary>
    /// 播放所有行为
    /// </summary>
    private void BehaviorActoring(int index)
    {
        if (!BehaviorDic.ContainsKey(index))
        {
            Debug.LogFormat("未发现第{0}回合里的任何操作,正在规划中...", index);

        }
        else
        {
            var beList = BehaviorDic[index];//第index回合的所有行为 
            BehaviorOneStepAction(beList, 0);
            BehaviorOneStepAction(beList, 1);
            //第0步的所有玩家行为
            //第1步的所有玩家行为
        }
    }
    private void BehaviorOneStepAction(List<PlayerControlBehavior> playerControlBehaviors,int controlIndex)
    {
        var list = playerControlBehaviors.FindAll(x => x.ControlIndex == controlIndex);
        if (list.Count < 2)
        {
            var needFuncPlayer = playerlist.Find(x => x.Data.Id != list[0].BehaviorPlayerId);
            if(needFuncPlayer != null)
            {
                int id = needFuncPlayer.Data.Id;
                var cardData = RandomUseCard(needFuncPlayer);
                OnMsgUseCard(id, cardData);
                List<GameCell> cells = new List<GameCell>();
                switch (cardData.CardType)
                {
                    case CardType.Move:
                        cells = MoveBehavior(id, cardData);                        
                        break;
                    case CardType.Attack:
                        cells = AttackBehavior(id, cardData);
                        break;
                    default:
                        break;
                }
                var cell = RandomClickGameCell(cells);
                OnMsgClickCell(id, cell.Index, cell.CellType);
                Debug.LogFormat("玩家{0} 执行了类型为{1}，触发格子{2} 类型为{3}", id, cardData.CardType, cell.Index, cell.CellType);
            }             
        }
    }

    private void SpawnPlayer(int id,int index,PlayerType type,CellType cellType)
    {
        PlayerData data = new PlayerData();
        data.Hp = 3;
        data.CellType = cellType;
        data.PlayerType = type;
        data.Index = index;
        data.Id = id;
        data.BehaviorIndex = 2;
        SpawnCard(data);        
        
        var cell = gameCells.Find(x => x.Index == index && x.CellType == cellType);
        if (cell != null)
        {
            var playerObj = Instantiate(GamePlayerPrefab, cell.transform);
            var player = playerObj.GetComponent<GamePlayer>();
            player.SetGamePlayerData(data);
            playerlist.Add(player);
        }            
    }

    private void SpawnCard(PlayerData data)
    {
        var count = data.CellType == CellType.Down ? DownCellColumn * DownCellRow : UpCellColumn * UpCellRow;
        for (int i = 0; i < count; i++)
        {
            CardData card = new CardData(i < count / 2 ? CardType.Move : CardType.Attack, data.Id, i, true);
            data.CardsList.Add(card);
        }        
    }

    #region Card
    /// <summary>
    /// 使用卡牌
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="cardData"></param>
    private void OnMsgUseCard(int playerId, CardData cardData)
    {
        var player = playerlist.Find(x => x.Data.Id == playerId);
        var cards = player.Data.CardsList;
        if (player.Data.BehaviorIndex > 0)
        {            
            bool success = false;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Equals(cardData))
                {
                    success = true;
                    break;
                }
            }
            if (success)
            {
                currentCardType = cardData.CardType;
                switch (cardData.CardType)
                {
                    case CardType.Move:
                        MoveBehavior(playerId, cardData);
                        break;
                    case CardType.Attack:
                        AttackBehavior(playerId, cardData);
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            Debug.Log("当前回合达到上限");
        }        
    }
    /// <summary>
    /// 移动卡牌逻辑
    /// </summary>
    private List<GameCell> MoveBehavior(int playerId,CardData data)
    {
        var clickCells = new List<GameCell>();
        var player = playerlist.Find(x => x.Data.Id == playerId);
        if (player != null)
        {
            var cells = gameCells.FindAll(x => x.CellType == player.Data.CellType);
            for (int i = 0; i < gameCells.Count; i++)
            {
                gameCells[i].SetClickState(false);
            }
            int column = player.Data.CellType == CellType.Down ? DownCellColumn : UpCellColumn;
            int right = player.Data.Index + column;
            if (right <= cells.Count)
            {
                cells[right].SetClickState(true);
                clickCells.Add(cells[right]);
            }
            int left = player.Data.Index - column;
            if (left > 0)
            {
                cells[left].SetClickState(true);
                clickCells.Add(cells[left]);
            }
            int up = player.Data.Index - 1;
            bool upSu = cells[up].row == cells[player.Data.Index].row;
            if (up > 0 && upSu)
            {
                cells[up].SetClickState(true);
                clickCells.Add(cells[up]);
            }            
            int down = player.Data.Index + 1;
            bool downSu = cells[down].row == cells[player.Data.Index].row;
            if (down <= cells.Count && downSu)
            {
                cells[down].SetClickState(true);
                clickCells.Add(cells[down]);
            }
            currentCardData = data;
        }
        return clickCells;
    }
    /// <summary>
    /// 攻击卡牌逻辑
    /// </summary>
    private List<GameCell> AttackBehavior(int playerId, CardData data)
    {
        var player = playerlist.Find(x => x.Data.Id == playerId);
        int index = player.Data.Index;
        if (BehaviorDic.ContainsKey(behaviorIndex))
        {
            var be = BehaviorDic[behaviorIndex].Find(x => x.BehaviorPlayerId == playerId && x.CardType == CardType.Move);
            if (be!=null)
            {
                index = be.NextIndex;
            }
        }
        if (player != null)
        {
            tempAttackArea.Clear();
            int up = index - 1;
            var playerCell = gameCells.Find(x => x.Index == index && x.CellType == player.Data.CellType);
            int row = playerCell.row;
            for (int i = 0; i < gameCells.Count; i++)
            {
                if (gameCells[i].row == playerCell.row && !gameCells[i].Equals(playerCell))
                {
                    if(gameCells[i].CellType == playerCell.CellType)
                    {
                        if(gameCells[i].Index < playerCell.Index)
                        {
                            gameCells[i].SetClickState(true);
                            tempAttackArea.Add(gameCells[i]);
                        }
                    }
                    else
                    {
                        gameCells[i].SetClickState(true);
                        tempAttackArea.Add(gameCells[i]);
                    }                    
                }
                else
                {
                    gameCells[i].SetClickState(false);
                }
            }
            currentCardData = data;
        }
        return tempAttackArea;
    }
    #endregion

    #region Cell
    /// <summary>
    /// 点击格子
    /// </summary>
    private void OnMsgClickCell(int playerId,int index,CellType cellType)
    {
        var cell = gameCells.Find(x => x.Index == index && x.CellType == cellType);
        var player = playerlist.Find(x => x.Data.Id == playerId);
        PlayerControlBehavior be = new PlayerControlBehavior();
        switch (currentCardType)
        {            
            //BehaviorDic
            case CardType.Move:
                if (cell.CellType != player.Data.CellType) return;                
                be.BehaviorPlayerId = playerId;
                be.CardType = CardType.Move;
                be.CellType = cellType;
                be.DefaultIndex = player.Data.Index;
                be.NextIndex = index;
                be.ControlIndex = 2 - player.Data.BehaviorIndex;
                break;
            case CardType.Attack:
                be.BehaviorPlayerId = playerId;
                be.CardType = CardType.Attack;
                be.CellType = cellType;
                be.DefaultIndex = player.Data.Index;
                be.NextIndex = index;
                be.AttackAreas = tempAttackArea;
                be.ControlIndex = 2 - player.Data.BehaviorIndex;
                break;
            default:
                break;
        }
        //清除所有可以点的格子，当前操作行为结束//TODO这里应该是数据变化，而不是显示变化
        for (int i = 0; i < gameCells.Count; i++)
        {
            gameCells[i].SetClickState(false);
        }
        if (BehaviorDic.ContainsKey(behaviorIndex))
        {
            BehaviorDic[behaviorIndex].Add(be);
        }
        else
        {
            var beList = new List<PlayerControlBehavior>();
            beList.Add(be);
            BehaviorDic.Add(behaviorIndex, beList);
        }
        player.Data.BehaviorIndex--;
        if (currentCardData != null)
        {
            currentCardData.CanUse = false;
            MessageCenter<int, CardData>.SendMessage((int)MsgEnum.BehaviorLimit, player.Data.BehaviorIndex, currentCardData);
        }
    }
    #endregion

    #region Ramdon
    private CardData RandomUseCard(GamePlayer player)
    {
        var cardList = player.Data.CardsList.FindAll(x => x.CanUse == true);
        int useCardIndex = UnityEngine.Random.Range(0, cardList.Count);
        return cardList[useCardIndex];
    }

    private GameCell RandomClickGameCell(List<GameCell> cells)
    {
        var index = UnityEngine.Random.Range(0, cells.Count);
        return cells[index];
    }
    #endregion

    private void FixedUpdate()
    {
        if (isGameDesign)
        {
            timer += Time.deltaTime;
            if (GameStartAction != null)
            {
                GameStartAction(timer);
            }
            if (timer >= overTime)
            {
                timer = 0;
                isGameDesign = false;
            }
        }
        else
        {
            timer = 0;
        }

    }
}

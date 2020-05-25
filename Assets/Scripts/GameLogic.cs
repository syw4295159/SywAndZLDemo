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
    [Space]
    [Header("Behavior")]
    public Dictionary<int, KeyValuePair<int, PlayerControlBehavior>> BehaviorDic = new Dictionary<int, KeyValuePair<int, PlayerControlBehavior>>();


    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        MessageCenter.RegisterMessage((int)MsgEnum.StepOneOver, StepOneOver);
        MessageCenter.RegisterMessage((int)MsgEnum.StepTimeOver, StepTimeOver);

        MessageCenter<int, CardData>.RegisterMessage((int)MsgEnum.UseCard, OnMsgUseCard);
    }
    private void Start()
    {
        fc = new FsmCore();
        gameCells.Clear();
        playerlist.Clear();
        SpawnCell(UpCellRow, UpCellColumn, CellType.Up);
        SpawnCell(DownCellRow, DownCellColumn, CellType.Down);

        StepOne();
        MessageCenter.SendMessage((int)MsgEnum.StepOne);
    }

    private void OnDisable()
    {
        MessageCenter.UnRegisterMessage((int)MsgEnum.StepOneOver, StepOneOver);
        MessageCenter.UnRegisterMessage((int)MsgEnum.StepTimeOver, StepTimeOver);
        MessageCenter<int, CardData>.UnRegisterMessage((int)MsgEnum.UseCard, OnMsgUseCard);
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
    private void StepOneOver()
    {
        Debug.Log("StepOneOver");
        MessageCenter.SendMessage((int)MsgEnum.StepTime);
        isGameDesign = true;
    }

    private void StepTimeOver()
    {
        Debug.Log("倒计时结束");
    }

    private void SpawnPlayer(int id,int index,PlayerType type,CellType cellType)
    {
        PlayerData data = new PlayerData();
        data.Hp = 3;
        data.CellType = cellType;
        data.PlayerType = type;
        data.Index = index;
        data.Id = id;
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

    /// <summary>
    /// 使用卡牌
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="cardData"></param>
    private void OnMsgUseCard(int playerId, CardData cardData)
    {
        var player = playerlist.Find(x => x.Data.Id == playerId);
        var cards = player.Data.CardsList;
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
            switch (cardData.CardType)
            {
                case CardType.Move:
                    MoveBehavior(playerId, cardData);
                    break;
                case CardType.Attack:
                    AttackBehavior(playerId,cardData);
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 移动卡牌逻辑
    /// </summary>
    private void MoveBehavior(int playerId,CardData data)
    {
            var player = playerlist.Find(x => x.Data.Id == playerId);
        if (player != null)
        {
            var cells = gameCells.FindAll(x => x.CellType == player.Data.CellType);
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].SetClickState(false);
            }
            int column = player.Data.CellType == CellType.Down ? DownCellColumn : UpCellColumn;
            int right = player.Data.Index + column;
            if (right <= cells.Count)
            {
                cells[right].SetClickState(true);
            }
            int left = player.Data.Index - column;
            if (left > 0)
            {
                cells[left].SetClickState(true);
            }
            int up = player.Data.Index - 1;
            bool upSu = cells[up].row == cells[player.Data.Index].row;
            if (up > 0 && upSu)
            {
                cells[up].SetClickState(true);
            }            
            int down = player.Data.Index + 1;
            bool downSu = cells[down].row == cells[player.Data.Index].row;
            if (down <= cells.Count && downSu)
            {
                cells[down].SetClickState(true);
            }
        }
    }
    /// <summary>
    /// 攻击卡牌逻辑
    /// </summary>
    private void AttackBehavior(int playerId, CardData data)
    {

    }

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
